using System;
using Halifax.Tests.Samples.ATM.Domain.Accounts;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash
{
    [Serializable]
    public class CashDeposited : AccountChanged
    {
        public decimal DepositAmount { get; set; }

    	public CashDeposited()
        {

        }

        public CashDeposited(string accountNumber, decimal depositAmount)
        {
            AccountNumber = accountNumber;
            DepositAmount = depositAmount;
        }
    }
}