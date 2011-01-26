using System;
using Halifax.Commanding;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount
{
    [Serializable]
    public class CreateAccountCommand : Command
    {
        public string Teller { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal InitialAmount { get; set; }

        public CreateAccountCommand()
            :this(string.Empty,  string.Empty, string.Empty, 0.00M)
        {
        }

        public CreateAccountCommand(string teller, string firstname, string lastname, decimal initialAmount)
        {
            Teller = teller;
            FirstName = firstname;
            LastName = lastname;
            InitialAmount = initialAmount;
        }

    }
}