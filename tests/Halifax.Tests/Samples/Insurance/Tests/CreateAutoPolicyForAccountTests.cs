using System;
using System.Collections.Generic;
using Halifax.Events;
using Halifax.Testing;
using Halifax.Tests.Samples.Insurance.Domain;
using Halifax.Tests.Samples.Insurance.Domain.Auto;
using Halifax.Tests.Samples.Insurance.Domain.Marketing;
using Xunit;

namespace Halifax.Tests.Samples.Insurance.Tests
{
	public class when_an_existing_policy_holder_requests_an_auto_policy
		: BaseAggregateRootTestFixture<Account, CreateAutoPolicyCommand>
	{
		private Guid _accountNumber = Guid.Empty;
		private string _agent;
		private PolicyHolder _policyHolder;
		private Vehicle _vehicle;

		public override void Initially()
		{
			_accountNumber = Guid.NewGuid();
			_agent = Guid.NewGuid().ToString();

			_policyHolder = new PolicyHolder
								{
									Name = new Name("joe", "", "smith"),
									DOB = "04-12-1967",
									SSN = "1234567890"
								};

			_vehicle = new Vehicle("Honda", "Accord", 2005);
		}

		public override IEnumerable<Event> Given()
		{
			// create the account with an agent and policy holder:
			yield return new AccountCreatedEvent(_agent, _policyHolder);
		}

		public override CreateAutoPolicyCommand When()
		{
			// create the auto policy on the account for the policy holder:
			return new CreateAutoPolicyCommand
					{
						AccountNumber = _accountNumber,
						PrimaryDriver = _policyHolder.Name,
						Vehicle = _vehicle,
						PolicyId = AggregateRoot.Id
					};
		}

		[Fact]
		public void it_will_denote_an_event_indicating_that_an_auto_policy_was_added_to_the_account()
		{
			PublishedEvents.Latest<AutoPolicyCreatedEvent>().AccountNumber = _accountNumber;
			PublishedEvents.Latest<AutoPolicyCreatedEvent>().Agent = _agent;
			PublishedEvents.Latest<AutoPolicyCreatedEvent>().PrimaryDriver = _policyHolder.Name;
			PublishedEvents.Latest<AutoPolicyCreatedEvent>().Vehicle = _vehicle;
		}

	}
}