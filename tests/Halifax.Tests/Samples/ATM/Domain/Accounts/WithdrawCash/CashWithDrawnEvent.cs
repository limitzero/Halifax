using System;
using Halifax.Tests.Samples.ATM.Domain.Accounts;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash
{
    /// <summary>
    /// Event denoting that cash has been withdrawn for an account.
    /// </summary>
    [Serializable]
    public class CashWithdrawnEvent : AccountChangedEvent
    {
        public decimal WithdrawalAmount { get; set; }

        public CashWithdrawnEvent()
        {
            
        }

        public CashWithdrawnEvent(string accountNumber, decimal withdrawalAmount)
        {
            AccountNumber = accountNumber;
            WithdrawalAmount = withdrawalAmount;
        }
    }
}