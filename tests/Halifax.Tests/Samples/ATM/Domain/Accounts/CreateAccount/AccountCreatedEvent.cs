using System;
using Halifax.Tests.Samples.ATM.Domain.Accounts;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount
{
    [Serializable]
    public class AccountCreatedEvent : AccountChangedEvent
    {
        public string Teller { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal InitialAmount { get; set; }

        public AccountCreatedEvent()
        {
        }

        public AccountCreatedEvent(string teller, string firstName, string lastName)
            :this(teller, firstName, lastName, 0.0M)
        {
       
        }

        public AccountCreatedEvent(string teller, string firstName, string lastName, decimal initialAmount)
        {
            Teller = teller;
            FirstName = firstName;
            LastName = lastName;
            InitialAmount = initialAmount;
        }
    }
}