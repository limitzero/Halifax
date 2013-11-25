using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Halifax.Commands;
using Halifax.Events;
using Halifax.StateMachine.Impl;

namespace Halifax.StateMachine
{
	/// <summary>
	/// Marker interface for state machines for container.
	/// </summary>
	public interface IStateMachine : IDisposable
	{
		/// <summary>
		/// Gets or sets the current state of the state machine when 
		///  state machine's instance data is retreived from the persistance 
		///  store.
		/// </summary>
		State CurrentState { get; set; }

		/// <summary>
		/// Gets the set of events that are to be emitted after a message is 
		/// received and the business logic determines that an event should 
		/// be issued.
		/// </summary>
		IEnumerable<Event> Events { get; }

		/// <summary>
		/// Gets the set of commands that are to be emitted after a message is 
		/// received and the business logic determines that a command should 
		/// be issued.
		/// </summary>
		IEnumerable<Command> Commands { get; }

		/// <summary>
		/// Gets or sets the indicator as to whether the process is completed.
		/// </summary>
		bool IsCompleted { get; set; }
	}

	/// <summary>
	/// The state machine represents a longer running process that 
	/// inspects events that are emitted in the system and can send 
	/// commands or publish other events to accomplish the process
	/// over a given period. An implementation restriction is that 
	/// state machines only respond to events. This can be used 
	/// as an implementation of the Process Manager EIP (Enterprise
	/// Integration Pattern)
	/// </summary>
	/// <typeparam name="TStateMachineData">The data that is persisted after each invocation to the state machine</typeparam>
	public abstract class StateMachine<TStateMachineData> : IStateMachine
		where TStateMachineData : class, IStateMachineData, new()
	{
		/// <summary>
		/// Gets or sets the data that is used by the state machine and persisted after 
		/// each call to handle a message. When <seealso cref="IsCompleted"/> is 
		/// set to true, the data that is correlated to this instance of the state machine 
		/// will be released.
		/// </summary>
		public TStateMachineData Data { get; set; }

		private List<Event> events;

		/// <summary>
		/// Gets or sets the current state of the state machine when 
		///  state machine's instance data is retreived from the persistance 
		///  store.
		/// </summary>
		public State CurrentState { get; set; }
	
		public IEnumerable<Event> Events
		{
			get { return events; }
		}

		private List<Command> commands;
		public IEnumerable<Command> Commands
		{
			get { return commands; }
		}

		public bool IsCompleted { get; set; }

		public List<StateMachineDataToMessageDataCorrelation> Correlations { get; private set; }

		protected StateMachine()
		{
			this.Data = new TStateMachineData();
			this.Correlations = new List<StateMachineDataToMessageDataCorrelation>();
			this.events = new List<Event>();
			this.commands = new List<Command>();
			this.InitializeStates();
		}

		public void Dispose()
		{
			this.Disposing(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// This sets up correlations, or how the state machine can find
		/// the state machine data from the characteristics on the incoming 
		/// message. It could be any data element that you like as long 
		/// as subsequent messages hold the same data element value
		/// between requests (i.e. account number, confirmation number, etc)
		/// a "business identifier" if needed.
		/// </summary>
		/// <typeparam name="TMessage">Message to be processed on state machine</typeparam>
		/// <param name="currentMessageDataKey">Expression denoting the property on the message to inspect</param>
		/// <param name="stateMachineDataKey">Expression denoting the property on the state machine data to inspect</param>
		protected void CorrelatedBy<TMessage>(
			Expression<Func<TMessage, object>> currentMessageDataKey,
			Expression<Func<TStateMachineData, object>> stateMachineDataKey)
			where TMessage : Event
		{
			string stateMachineDataPropertyName = GetPropertyNameFromExpression(stateMachineDataKey);
			string eventMessageDataPropertyName = GetPropertyNameFromExpression(currentMessageDataKey);

			var correlation =
				new StateMachineDataToMessageDataCorrelation(typeof(TMessage),
															 stateMachineDataPropertyName,
															 eventMessageDataPropertyName);

			if (this.Correlations.Contains(correlation) == false)
			{
				this.Correlations.Add(correlation);
			}
		}

		/// <summary>
		/// Examines the current state on the contract <seealso cref="IStateMachineData.State"/>
		/// against another independently created state on the state machine.
		/// </summary>
		/// <returns></returns>
		protected bool IsCurrentStateEqualTo(State state)
		{
			return new State { Name = this.Data.State } == state;
		}

		/// <summary>
		/// Enqueues an on-demand event for distribution after the current 
		/// event on the state machine has been processed.
		/// </summary>
		/// <param name="event"></param>
		public void AddEvent(Event @event)
		{
			this.events.Add(@event);
		}

		/// <summary>
		/// Enqueues an on-demand command for distribution after the current 
		/// event on the state machine has been processed. Limitation: one and only 
		/// one command can be issued per call to enqueue a command for dispatch.
		/// </summary>
		/// <param name="command"></param>
		public void AddCommand(Command command)
		{
			if (this.commands.Contains(command) == false)
				this.commands.Add(command);
		}

		/// <summary>
		/// Transitions the state machine to a defined state for inspection when 
		/// certain conditions are met.
		/// </summary>
		/// <param name="state"></param>
		public void TransitionTo(State state)
		{
			if (state != null)
			{
				this.Data.State = state.Name;
				this.CurrentState = state;
			}
		}

		/// <summary>
		/// This will mark the current statemachine as having completed its processing and 
		/// all created state for this instance will be destroyed in the underlying persistance store
		/// </summary>
		public void MarkAsCompleted()
		{
			this.IsCompleted = true;
		}

		public virtual void Disposing(bool disposing)
		{
			if (disposing == true)
			{
				if (this.Correlations != null)
				{
					this.Correlations.Clear();
				}
				this.Correlations = null;

				if (this.events != null)
				{
					this.events.Clear();
				}
				this.events = null;

				if (this.commands != null)
				{
					this.commands.Clear();
				}
				this.commands = null;

				this.Data = null;
			}
		}

		private void InitializeStates()
		{
			var declaredStatesAsProperties = this.GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => typeof(State).IsAssignableFrom(m.PropertyType))
				.ToList();

			foreach (var declaredStateAsProperty in declaredStatesAsProperties)
			{
				var state = new State { Name = declaredStateAsProperty.Name };
				declaredStateAsProperty.SetValue(this, state, null);
			}

			this.CurrentState = new State{ Name = "Initial"};
		}

		private static string GetPropertyNameFromExpression<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			MemberExpression memberExpression;

			if (expression.Body is UnaryExpression)
			{
				memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
			}
			else
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new InvalidOperationException("You must specify a property!");
			}

			return memberExpression.Member.Name;
		}

	}
}