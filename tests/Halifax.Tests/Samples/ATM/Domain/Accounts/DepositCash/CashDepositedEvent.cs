using System;
using Halifax.Tests.Samples.ATM.Domain.Accounts;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash
{
    [Serializable]
    public class CashDepositedEvent : AccountChangedEvent
    {
        public decimal DepositAmount { get; set; }

        public CashDepositedEvent()
        {

        }

        public CashDepositedEvent(string accountNumber, decimal depositAmount)
        {
            AccountNumber = accountNumber;
            DepositAmount = depositAmount;
        }
    }
}