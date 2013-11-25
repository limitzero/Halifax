using System;
using Halifax.Commands;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash
{
    [Serializable]
    public class DepositCashCommand : Command
    {

        public Guid Id { get; set; }
        public decimal DepositAmount { get; set; }

    	public string AccountNumber { get; set; }

    	public DepositCashCommand()
            :this(Guid.Empty, decimal.Zero)
        {
        }

		public DepositCashCommand(string accountNumber, decimal depositAmount)
			:this(Guid.Empty, depositAmount)
		{
			AccountNumber = accountNumber;
		}

        public DepositCashCommand(Guid id, decimal depositAmount)
        {
            Id = id;
            DepositAmount = depositAmount;
        }

    }
}