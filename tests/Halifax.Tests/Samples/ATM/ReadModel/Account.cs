using System;
using Halifax.Read;

namespace Halifax.Tests.Samples.ATM.ReadModel
{
	// Basic projection from the "Account" aggregate root:
	public class Account : IReadModel
	{
		public Guid Id { get; set; }
		public string AccountNumber { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public decimal Balance { get; set; }
	}
}