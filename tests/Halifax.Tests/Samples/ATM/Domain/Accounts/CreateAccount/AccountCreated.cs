using System;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount
{
	[Serializable]
	public class AccountCreated : AccountChanged
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public decimal InitialAmount { get; set; }

		public AccountCreated()
		{
		}

		public AccountCreated(string firstName, string lastName, decimal initialAmount)
		{
			FirstName = firstName;
			LastName = lastName;
			InitialAmount = initialAmount;
		}
	}
}