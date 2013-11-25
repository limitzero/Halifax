using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Tests.Samples.ATM.Domain.Accounts;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash;

namespace Halifax.Tests.Samples.ATM.ReadModel
{
	[Obsolete]
    public class ReadModelDB
    {
        private static List<AccountChanged> _account_transactions;

        static ReadModelDB()
        {
            if(_account_transactions == null)
                _account_transactions = new List<AccountChanged>();
        }

        public static void Refresh()
        {
            _account_transactions.Clear();
        }

        public static void RecordTransaction(AccountChanged @event)
        {
            _account_transactions.Add(@event);
        }

        public static AccountBalanceView GetAccountBalance(Guid Id)
        {
            var view = new AccountBalanceView();

            var balance = decimal.Zero;

            // always build the account balance from the bottom-up (ascending listing);
        	var transactions = GetAllTransactions(Id);

            // playback all transactions to get the right account balance:
            foreach(var transaction in transactions)
            {
                if (transaction is CashDeposited)
                    balance += (transaction as CashDeposited).DepositAmount;

                if (transaction is CashWithdrawn)
                    balance -= (transaction as CashWithdrawn).WithdrawalAmount;
            }

            // map to view model:
        	view.AccountNumber = Id.ToString();
            view.CurrentBalance = balance;

            return view;
        }

		private static List<AccountChanged> GetAllTransactions(Guid accountId)
		{
			// always build the account balance from the bottom-up (ascending listing);
			var transactions = (from transaction in _account_transactions
								let transactionDate = transaction.At
								where transaction.AccountNumber == accountId.ToString()
								orderby transactionDate ascending
								select transaction).Distinct().ToList();
			return transactions;
		}

    	~ReadModelDB()
        {
            _account_transactions = null;
        }
    }
}