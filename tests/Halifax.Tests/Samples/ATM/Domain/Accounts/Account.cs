using System;
using Halifax.Domain;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.Domain.Accounts.Exceptions;
using Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash;
using Halifax.Tests.Samples.ATM.Services;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts
{
    public class Account : AggregateRoot
    {
    	private readonly IOverdraftInspectionService overdraftInspectionService;

    	public Account(IOverdraftInspectionService overdraftInspectionService)
    	{
    		this.overdraftInspectionService = overdraftInspectionService;
    	}

    	public void Create(string firstName, string lastName, decimal  initialAmount)
        {
			// UC1: create the account and assign a business specific account number for compliance purposes.
        	var accountNumber = CombGuid.NewGuid().ToString();
			var ev = new AccountCreated(firstName, lastName, initialAmount) { AccountNumber = accountNumber };
            Apply(ev);
        }

        public void DepositCash(string accountNumber, decimal  depositAmount)
        {
			var ev = new CashDeposited(accountNumber, depositAmount);
            Apply(ev);
        }

        public void WithdrawCash(string accountNumber, decimal  withdrawalAmount)
        {
            InspectBalanceAgainstWithdrawalAmount(accountNumber, withdrawalAmount);
			var ev = new CashWithdrawn(accountNumber, withdrawalAmount);
            Apply(ev);
        }

		private void InspectBalanceAgainstWithdrawalAmount(string accountNumber, decimal withdrawalAmount)
        {
            // UC2: when the withdrawal amount exceeds the current balance
            // generate an exception indicating as such to the customer:
			decimal balance = decimal.Zero;
			if (this.overdraftInspectionService.IsOverdrawn(accountNumber, withdrawalAmount, out balance))
				throw new WithdrawalAmountExceedsAvaliableFundsException(withdrawalAmount, balance);
        }
    }
}