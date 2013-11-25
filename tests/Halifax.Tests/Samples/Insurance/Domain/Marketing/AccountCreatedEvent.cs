using System;

namespace Halifax.Tests.Samples.Insurance.Domain.Marketing
{
	public class AccountCreatedEvent : AccountChangedEvent
	{
		public string Agent { get; set; }
		public PolicyHolder PolicyHolder { get; set; }

		public AccountCreatedEvent(string agent, PolicyHolder policyHolder)
		{
			Agent = agent;
			this.PolicyHolder = policyHolder;
		}
	}
   
}