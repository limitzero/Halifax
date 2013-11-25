using System;
using Halifax.Events;

namespace Halifax.Tests.Samples.Insurance.Domain.Auto
{
	public class AutoPolicyCreatedEvent : Event
	{
		public Guid AccountNumber { get; set; }
		public Name PrimaryDriver { get; set; }
		public Vehicle Vehicle { get; set; }
		public string Agent { get; set; }
	}
}