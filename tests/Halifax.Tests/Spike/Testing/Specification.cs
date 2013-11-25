using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Halifax.Commands;
using Halifax.Configuration;
using Halifax.Configuration.Impl.Eventing;
using Halifax.Domain;
using Halifax.Events;
using Halifax.Internals.Commands;
using Halifax.Internals.Exceptions;

namespace Halifax.Tests.Spike.Testing
{
	/// <summary>
	/// Runs all specifications in a given assembly.
	/// </summary>
	public class SpecificationRunner
	{
		public void Run()
		{
			var specifications = this.GetType().Assembly.GetTypes()
				.Where(t => typeof(BaseSpecification).IsAssignableFrom(t))
				.Where(t => t.IsClass == true)
				.Where(t => t.IsAbstract == false)
				.Select(t => this.GetType().Assembly.CreateInstance(t.FullName) as BaseSpecification)
				.ToList();

			foreach (var specification in specifications)
			{
				specification.Execute();
			}
		}
	}

	/// <summary>
	/// Base specification class to test the behavior of an aggregate root. 
	/// </summary>
	/// <typeparam name="TAggregateRoot">Type of the class representing the aggregate root.</typeparam>
	public  abstract class Specification<TAggregateRoot> : BaseSpecification where TAggregateRoot : AggregateRoot
	{
		private TAggregateRoot aggregate_root;
		private readonly IConfiguration configuration;

		/// <summary>
		/// Gets the identifier of the aggregate root in the current testing scenario.
		/// </summary>
		public Guid Id { get; private set; }

		protected Specification()
		{
			this.configuration = new Halifax.Configuration.Impl.Configuration();
			this.Id = CombGuid.NewGuid();
		}

		public override void Execute()
		{
			// configure the infrastructure to pick up the 
			// corresponding elements to complete testing:
			this.BuildConfiguration();

			// create the aggregate from the repository and use 
			// it for the basis of verifiying the specification:
			this.FetchAggregate();

			// load the history on the aggregate from the series of 
			// previously defined events in the specification:
			this.LoadAggregateHistory();

			// get the current command that should trigger the actions 
			// against the aggregate:
			Command command = this.When;

			// issue the command against the aggreate:
			try
			{
				var command_bus = this.configuration.CurrentContainer().Resolve<ICommandBus>();
				command_bus.Send(command);
			}
			catch (Exception e)
			{
				if (typeof(HalifaxException).IsAssignableFrom(e.GetType()))
					throw e;

				CaughtException = e;
			}
			finally
			{
				if (this.Finally != null)
					Finally();
			}

			this.Verbalize();
		}

		private void FetchAggregate()
		{
			this.aggregate_root = this.configuration.CurrentContainer()
			.Resolve<IAggregateRootRepository>().Get<TAggregateRoot>(this.Id);
		}

		private void LoadAggregateHistory()
		{
			IEnumerable<Event> changes = Given();
			if (changes == null) return;
			if (changes.Count() == 0) return;

			this.aggregate_root.LoadFromHistory(changes);

			// push the events out to the corresponding handlers (if defined):
			var event_bus = this.configuration.CurrentContainer().Resolve<IEventBus>();
			foreach (var change in changes)
				event_bus.Publish(change);
		}

		private void BuildConfiguration()
		{
			// build the volatile version of the elements for testing:
			this.configuration
				.Container(c => c.UsingCastleWindsor())
				.Eventing(ev => ev.Synchronous())
				.EventStore(es => es.UsingInMemoryStorage())
				.Serialization(s => s.UsingSharpSerializer())
				.ReadModel(rm => rm.UsingInMemoryRepository())
				.Configure(this.GetType().Assembly);
		}

		private bool Verbalize()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine(this.GetType().Name.Replace("_", " "));
			this.VerbalizeGiven(builder);
			this.VerbalizeWhen(builder);
			bool success = this.VerbalizeExpected(builder);
			System.Console.WriteLine(builder.ToString());
			return success;
		}

		private void VerbalizeGiven(StringBuilder builder)
		{
			if(this.Given() != null)
			{
				var pre_conditions = this.Given();

				if (pre_conditions != null && pre_conditions.Count() > 0)
				{
					builder.AppendLine("Given:");

					foreach (var pre_condition in pre_conditions)
					{
						builder.AppendFormat("\t{0}", this.ReflectOverMessage(pre_condition)).AppendLine();
					}
				}
			}
		}

		private void VerbalizeWhen(StringBuilder builder)
		{
			builder.Append("When: ")
				.AppendFormat("{0}", this.ReflectOverMessage(this.When)).AppendLine();
		}

		private bool VerbalizeExpected(StringBuilder builder)
		{
			bool success = true;
			var registered_events = this.aggregate_root.GetChanges();

			builder.AppendLine("Expected");

			foreach (var @event in Expect)
			{
				var registered_event = registered_events.FirstOrDefault(ev => ev.GetType().Name.Equals(@event.GetType().Name));
				string result = string.Empty;

				//if (registered_event != null && ReferenceEquals(registered_event, @event) == true)
				if (registered_event != null && (registered_event.GetType() == @event.GetType()))
				{
					result = "Passed";
				}
				else
				{
					result = "Failed";
					success = false;
				}

				builder.AppendFormat("\t[{0}]: {1}", result, this.ReflectOverMessage(@event)).AppendLine();
			}

			return success;
		}

		private string ReflectOverMessage(object message)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(message.GetType().Name).Append(" {");
			var properties = message.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in properties)
			{
				builder.AppendFormat("{0} = {1}, ", property.Name, property.GetValue(message, null).ToString());
			}

			string data = builder.ToString().TrimEnd(new char[] {',', ' '}).Trim();

			data +=" }";

			return data;
		}
	}

	/// <summary>
	/// Base class to implement the "Given-When-Then(Expect)" structure for testing 
	/// commands against the aggregates and the corresponding events that 
	/// are triggered. 
	/// </summary>
	public abstract class BaseSpecification
	{
		/// <summary>
		/// The exception that is caught as a result of executing the command 
		/// against the aggregate.
		/// </summary>
		public Exception CaughtException { get; set; }

		/// <summary>
		/// Gets or sets the setup actions for the specification.
		/// </summary>
		public Action Before { get; set; }

		/// <summary>
		/// This will return a series of events to hydrate the state 
		/// of the aggregate before the command is issued to change
		/// its underlying state.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<Event> Given()
		{
			return null;
		}

		/// <summary>
		/// Gets or sets the command to trigger to the domain for a series of actions 
		/// (i.e. "events") to take place.
		/// </summary>
		public Command When { get; set; }

		/// <summary>
		/// Gets or sets the collection of events that should be produced 
		/// from the aggregate as a result of the command being issued.
		/// </summary>
		public IEnumerable<Event> Expect { get; set; }

		/// <summary>
		/// Gets or sets the tear-down action for the spectification.
		/// </summary>
		public Action Finally { get; set; }

		public abstract void Execute();
	}
}