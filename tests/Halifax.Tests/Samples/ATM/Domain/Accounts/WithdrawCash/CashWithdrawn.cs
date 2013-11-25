using System;
using Halifax.Tests.Samples.ATM.Domain.Accounts;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash
{
    /// <summary>
    /// Event denoting that cash has been withdrawn for an account.
    /// </summary>
    [Serializable]
    public class CashWithdrawn : AccountChanged
    {
        public decimal WithdrawalAmount { get; set; }

        public CashWithdrawn()
        {
            
        }

        public CashWithdrawn(string accountNumber, decimal withdrawalAmount)
        {
            AccountNumber = accountNumber;
            WithdrawalAmount = withdrawalAmount;
        }
    }
}