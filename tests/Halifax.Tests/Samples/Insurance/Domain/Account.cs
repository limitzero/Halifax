using System;
using System.Collections.Generic;
using Halifax.Domain;
using Halifax.Tests.Samples.Insurance.Domain.Auto;
using Halifax.Tests.Samples.Insurance.Domain.Marketing;

namespace Halifax.Tests.Samples.Insurance.Domain
{
	/// <summary>
	/// An account is the primary method of recording the individual or inviduals
	/// who have signed up with the insurance agency and who have policies for 
	/// securing certain assets.
	/// </summary>
	public class Account : AggregateRoot
	{
		private string _agent;
		private Guid _accountNumber;

		// need to use domain services here for inspecting all of the types of policies:
		private readonly List<PolicyHolder> _policyHolders;
		private readonly List<AutoPolicy> _autoPolicies;

		public Account()
		{
			_policyHolders = new List<PolicyHolder>();
			_autoPolicies = new List<AutoPolicy>();
		}

		public void Create(string agent, PolicyHolder policyHolder)
		{
			var ev = new AccountCreatedEvent(agent, policyHolder);
			Apply(ev);
		}

		public void CreateAutoPolicy(Vehicle vehicle, Name primaryDriver, List<Name> drivers)
		{
			var ev = new AutoPolicyCreatedEvent
			         	{
			         		Agent = _agent,
			         		AccountNumber = _accountNumber,
			         		PrimaryDriver = primaryDriver,
			         		Vehicle = vehicle
			         	};
			Apply(ev);
		}

		private void OnAccountCreatedEvent(AccountCreatedEvent @event)
		{
			// change the state of the entity first, then set the account number on the event:
			_agent = @event.Agent;
			_accountNumber = Guid.NewGuid();
			_policyHolders.Add(@event.PolicyHolder);
			@event.AccountNumber = _accountNumber.ToString();
		}

		private void OnAutoPolicyCreatedEvent(AutoPolicyCreatedEvent domainEvent)
		{
			// add the auto policies to the list of policies for the account:
			var autoPolicy = new AutoPolicy
								{
									Vehicle = domainEvent.Vehicle,
									PrimaryDriver = domainEvent.PrimaryDriver
								};
			this._autoPolicies.Add(autoPolicy);
		}

	
	}
}