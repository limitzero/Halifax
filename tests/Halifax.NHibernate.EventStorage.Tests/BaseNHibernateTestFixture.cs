using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Commands;
using Halifax.Configuration.Impl.Eventing;
using Halifax.Domain;
using Halifax.Events;
using Halifax.Internals.Commands;
using Halifax.Internals.Exceptions;
using Halifax.Read;
using Halifax.Testing;

namespace Halifax.NHibernate.Tests
{
	/// <summary>
	/// Base class for testing commands with the aggregate root and corresponding event handlers.
	/// </summary>
	/// <typeparam name="TAggregateRoot">Type of the aggregate root to respond to the command</typeparam>
	/// <typeparam name="TCommand">The initial command to be sent to the aggregate root for changing the state of the behavioral model</typeparam>
	public abstract class BaseNHibernateAggregateRootTestFixture<TAggregateRoot, TCommand> :
		BaseNHibernateTestConfiguration
		where TCommand : Command
		where TAggregateRoot : AggregateRoot
	{
		private ICommandBus commandBus;
		private IEventBus eventBus;

		/// <summary>
		/// The current aggregate root under test that defines the behavioral 
		/// model of the core entity and its associated entities and/or services
		/// </summary>
		protected TAggregateRoot AggregateRoot;

		protected BaseNHibernateAggregateRootTestFixture()
		{
			InitializeInfrastructure();
			ExecuteContext();
		}

		/// <summary>
		/// (Read-Only). The currently caught exception as captured for 
		/// sending the command to the domain for state change.
		/// </summary>
		protected Exception CaughtException { get; set; }

		/// <summary>
		/// The current listing of events that are succesfully emitted by 
		/// the behavioral model (i.e. aggregate root).
		/// </summary>
		protected ThePublishedEvents PublishedEvents { get; private set; }

		/// <summary>
		/// This will send an additional command to the domain 
		/// after the initial command issued in the <seealso cref="When"/>
		/// pre-condition has been sent.
		/// </summary>
		/// <param name="command"></param>
		protected void SendAdditionalCommandOf(Command command)
		{
			try
			{
				commandBus.Send(command);
			}
			catch (Exception e)
			{
				if (typeof(HalifaxException).IsAssignableFrom(e.GetType()))
					throw e;

				//CaughtException = new TheCaughtException(e);
				CaughtException = e;
			}
		}

		/// <summary>
		/// This will set up the fixture for any dependencies 
		/// or pre-test data staging.
		/// </summary>
		public virtual void Initially()
		{
		}

		/// <summary>
		/// This will set up the initial domain events to be issued 
		/// against the domain aggregate in order to prepare 
		/// it for accepting the current comand from <see cref="When"/>
		/// case.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<Event> Given()
		{
			return new List<Event>();
		}

		/// <summary>
		/// This will intialize the context for changing the domain 
		/// aggregate via the command. 
		/// </summary>
		public abstract TCommand When();

		public virtual void Finally()
		{
		}

		/// <summary>
		/// This will execute a query against the read model and return the result inside of the query object.
		/// </summary>
		/// <typeparam name="TReadModel"></typeparam>
		/// <param name="query"></param>
		protected void ExecuteQueryOverReadModel<TReadModel>(IQueryExecuter<TReadModel> query)
			where TReadModel : class, IReadModel
		{
			var read_model_repository =
				this.Configuration.CurrentContainer()
				.Resolve<IReadModelRepository<TReadModel>>();

			read_model_repository.Query(query);
		}

		private void ExecuteContext()
		{
			CaughtException = new NoExceptionWasCaughtButOneWasExpectedException();

			Initially();

			// prepare the aggregate:
			PrepareAggregate();

			// create the command:
			TCommand command = When();

			// issue the command against the aggreate:
			try
			{
				commandBus.Send(command);
			}
			catch (Exception e)
			{
				if (typeof(HalifaxException).IsAssignableFrom(e.GetType()))
					throw e;

				//CaughtException = new TheCaughtException(e);
				CaughtException = e;
			}
			finally
			{
				Finally();
			}

			PublishedEvents = new ThePublishedEvents(this.Configuration.CurrentContainer(), command);
		}

		private void PrepareAggregate()
		{
			IEnumerable<Event> changes = Given();
			if (changes.Count() == 0) return;

			AggregateRoot = this.Configuration.CurrentContainer()
				.Resolve<IAggregateRootRepository>().Get<TAggregateRoot>(CombGuid.NewGuid());

			AggregateRoot.LoadFromHistory(Given());

			foreach (var change in changes)
			{
				change.From = AggregateRoot.GetType().FullName;
				this.eventBus.Publish(change);
			}
		}

		public virtual void InitializeInfrastructure()
		{
			BuildInfrastructure();

			// start the commading bus for distributing messages to the command handlers:
			commandBus = this.Configuration.CurrentContainer().Resolve<ICommandBus>();

			// start the eventing bus for distributing messages to the event handlers:
			eventBus = this.Configuration.CurrentContainer().Resolve<IEventBus>();
		}
	}
}