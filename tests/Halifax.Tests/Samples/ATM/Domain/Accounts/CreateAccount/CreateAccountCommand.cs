using System;
using Halifax.Commands;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount
{
    [Serializable]
    public class CreateAccountCommand : Command
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal InitialAmount { get; set; }

        public CreateAccountCommand()
            :this(string.Empty, string.Empty, decimal.Zero)
        {
        }

        public CreateAccountCommand(string firstname, string lastname, decimal initialAmount)
        {
            FirstName = firstname;
            LastName = lastname;
            InitialAmount = initialAmount;
        }

    }
}