using System;
using Halifax.Commanding;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash
{
    [Serializable]
    public class WithdrawCashCommand : Command
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal WithdrawalAmount { get; set; }

        public WithdrawCashCommand()
        {
            
        }

        public WithdrawCashCommand(Guid id, string accountNumber, decimal withdrawalAmount)
        {
            Id = id;
            AccountNumber = accountNumber;
            WithdrawalAmount = withdrawalAmount;
        }

    }
}