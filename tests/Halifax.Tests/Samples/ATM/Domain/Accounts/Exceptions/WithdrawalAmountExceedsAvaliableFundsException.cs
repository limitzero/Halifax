using System;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.Exceptions
{
    public class WithdrawalAmountExceedsAvaliableFundsException : ApplicationException
    {
        private const string message = "The amount {0:c} currently exceeds the available balance of {1:c} for your account.";

        public WithdrawalAmountExceedsAvaliableFundsException(decimal withdrawalAmount, decimal currentBalance)
            : base(string.Format(message, withdrawalAmount, currentBalance))
        {
            
        }
    }
}