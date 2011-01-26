using System;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.Domain.Accounts.Exceptions;
using Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts
{
    public class Account : AbstractAggregateRootByConvention
        //: AbstractAggregateRoot
    {
        private string _accountNumber;
        private string _firstname;
        private string _lastname;
        private decimal _balance;
        private string _teller;

        public override void RegisterEvents()
        {
            // use the events by convention on the aggregate:
            //RegisterEvent<AccountCreatedEvent>(OnAccountCreatedEvent);
            //RegisterEvent<CashDepositedEvent>(OnCashDepositedEvent);
            //RegisterEvent<CashWithdrawnEvent>(OnCashWithdrawnEvent);
        }

        public void Create(CreateAccountCommand command)
        {
            var ev = new AccountCreatedEvent(command.Teller, command.FirstName, 
                command.LastName, command.InitialAmount);
            ApplyEvent(ev);
        }

        public void MakeCashDeposit(DepositCashCommand command)
        {
            var ev = new CashDepositedEvent(this._accountNumber, command.DepositAmount);
            ApplyEvent(ev);
        }

        public void WithdrawCash(WithdrawCashCommand command)
        {
            InspectBalance(command);

            var ev = new CashWithdrawnEvent(command.AccountNumber, command.WithdrawalAmount);
            ApplyEvent(ev);
        }

        private void InspectBalance(WithdrawCashCommand command)
        {
            // UC2: when the withdrawal amount exceeds the current balance
            // generate an exception indicating as such to the customer:
            if (command.WithdrawalAmount > _balance)
                throw new WithdrawalAmountExceedsAvaliableFundsException(command.WithdrawalAmount, _balance);
        }

        public void OnAccountCreatedEvent(AccountCreatedEvent domainEvent)
        {
            // UC1: create the account and assign a business specific 
            // account number for compliance purposes.
            _accountNumber = Guid.NewGuid().ToString(); 
            _firstname = domainEvent.FirstName;
            _lastname = domainEvent.LastName;
            _teller = domainEvent.Teller;
            _balance = domainEvent.InitialAmount;
        }

        public void OnCashDepositedEvent(CashDepositedEvent domainEvent)
        {
            _balance += domainEvent.DepositAmount;
        }

        public void OnCashWithdrawnEvent(CashWithdrawnEvent domainEvent)
        {            
            _balance -= domainEvent.WithdrawalAmount;
        }

        /// <summary>
        /// Used only for testing in the absence of a read-model.
        /// </summary>
        /// <returns></returns>
        public decimal GetCurrentBalance()
        {
            return _balance;
        }

        /// <summary>
        /// Used only for testing in the absence of a read-model.
        /// </summary>
        /// <returns></returns>
        public string GetAccountNumber()
        {
            return _accountNumber;
        }



    }
}