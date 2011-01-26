using System;
using Halifax.Commanding;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash
{
    [Serializable]
    public class DepositCashCommand : Command
    {

        public Guid Id { get; set; }
        public decimal DepositAmount { get; set; }

        public DepositCashCommand()
            :this(Guid.Empty, decimal.Zero)
        {
        }

        public DepositCashCommand(Guid id, decimal depositAmount)
        {
            Id = id;
            DepositAmount = depositAmount;
        }

    }
}