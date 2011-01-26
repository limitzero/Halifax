using System;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.Exceptions
{
    public class WithdrawalAmountExceedsAvaliableFundsException : ApplicationException
    {
        private const string _message = "The amount {0} currently exceeds the available balance of {1} for your account.";

        public WithdrawalAmountExceedsAvaliableFundsException(decimal withdrawalAmount, decimal currentBalance)
            : base(string.Format(_message, withdrawalAmount.ToString("$####.00"), currentBalance.ToString("$####.00")))
        {
            
        }
    }
}