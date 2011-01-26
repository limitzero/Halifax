using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Tests.Samples.ATM.Domain.Accounts;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash;

namespace Halifax.Tests.Samples.ATM.ReadModel
{
    public class ReadModelDB
    {
        private static List<AccountChangedEvent> _account_transactions;

        static ReadModelDB()
        {
            if(_account_transactions == null)
                _account_transactions = new List<AccountChangedEvent>();
        }

        public static void Refresh()
        {
            _account_transactions.Clear();
        }

        public static void RecordTransaction(AccountChangedEvent @event)
        {
            _account_transactions.Add(@event);
        }

        public static AccountBalanceView GetAccountBalance(Guid Id)
        {
            var view = new AccountBalanceView();

            var balance = decimal.Zero;

            // always build the account balance from the bottom-up (ascending listing);
            var transactions = (from transaction in _account_transactions
                                let transactionDate = transaction.EventDateTime
                                where transaction.AccountNumber == Id.ToString()
                                orderby transactionDate ascending 
                                select transaction).Distinct().ToList();

            // playback all transactions to get the right account balance:
            //foreach (var transaction in _account_transactions)
            foreach(var transaction in transactions)
            {
                if (transaction is CashDepositedEvent)
                    balance += (transaction as CashDepositedEvent).DepositAmount;

                if (transaction is CashWithdrawnEvent)
                    balance -= (transaction as CashWithdrawnEvent).WithdrawalAmount;
            }

            // map to view model:
            view.CurrentBalance = balance;

            return view;
        }

        ~ReadModelDB()
        {
            _account_transactions = null;
        }
    }
}