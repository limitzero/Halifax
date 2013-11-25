using System;
using Halifax.Events;
using Halifax.StateMachine;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts
{
	public class AccountStateMachineData : IStateMachineData
	{
		public Guid Id { get; set; }
		public string AccountNumber { get; set; }
		public string State { get; set; }
	}

	/// <summary>
	/// This can function as the process manager or coordination 
	/// of events to command on accessing differing aggregate 
	/// roots based on the messages being passed. The state 
	/// machine should only listen for events and send events/commands.
	/// </summary>
	public class AccountStateMachine : 
		StateMachine.StateMachine<AccountStateMachineData>,
		 EventConsumer.For<CashWithdrawn>,
		 EventConsumer.For<CashDeposited>
	{
		public State Started { get; set; }

		public AccountStateMachine()
		{
			// the account number will be the business key to correlate invocations around:
			CorrelatedBy<CashDeposited>(m => m.AccountNumber, d => d.AccountNumber);
			CorrelatedBy<CashWithdrawn>(m => m.AccountNumber, d => d.AccountNumber);
		}

		public void Handle(CashDeposited @event)
		{
			this.Data.AccountNumber = @event.AccountNumber;
			TransitionTo(this.Started);
			System.Diagnostics.Debug.WriteLine("State machine handled " + @event.GetType().FullName + " State => " + this.Data.State);
		}

		public void Handle(CashWithdrawn @event)
		{	
			// can check the state of current process before attempting
			// to delegate messages to aggregates for further processing:
			// (current state will always be the state of the instance data 
			// when retrieved from persistant storage or "Initial" on first 
			// access before storing):
			if(this.CurrentState == this.Started)
			{
				// remember, no mutation of data should occur in the state machine
				// only querying of data and sending corresponding events/commands
				// as a result of the query (domain services can help here with complicated 
				// querying of disparate aggregates):
			}
		}
	}
}