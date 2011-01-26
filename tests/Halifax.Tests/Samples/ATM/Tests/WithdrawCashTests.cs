using System.Collections.Generic;
using Halifax.Eventing;
using Halifax.Testing;
using Halifax.Tests.Samples.ATM.Domain.Accounts;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.Domain.Accounts.Exceptions;
using Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash;
using Xunit;

namespace Halifax.Tests.Samples.ATM.Tests
{
    /// <summary>
    /// UC1: when the customer attempts to withdraw cash from the atm and the account 
    /// has enough funds availaible to process the transaction, then debit the account by 
    /// the withdrawal amount and allocate the cash to the customer.
    /// </summary>
    public class when_withdrawing_cash_from_the_atm_and_the_account_has_the_available_funds : 
        BaseAggregateWithCommandConsumerTestFixture<Account, 
            WithdrawCashCommand, 
            WithdrawCashCommandConsumer>
    {
        private const string _teller = "Chuck Knorris";
        private readonly string _accountNumber = "1234567890";
        private const decimal _avaliableFunds = 150.00M;
        private const decimal _withdrawalAmount = 50.00M;

        public override IEnumerable<IDomainEvent> Given()
        {
            // create the account and seed it with some money:
            yield return new AccountCreatedEvent(_teller,"Joe", "Smith");
            yield return new CashDepositedEvent(_accountNumber, _avaliableFunds);
        }

        public override WithdrawCashCommand When()
        {
            // withdraw the cash from the account
            return new WithdrawCashCommand(Aggregate.Id, _accountNumber,_withdrawalAmount);
        }

        [Fact]
        public void it_will_publish_an_event_noting_that_the_cash_has_been_withdrawn()
        {
            PublishedEvents.Latest().WillBeOfType<CashWithdrawnEvent>();
        }

    }

    /// <summary>
    /// UC2: When the customer is attempting to withdraw an amount that exceeds the 
    /// current balance, indicate to the customer that the withdrawal can not be 
    /// completed on the account for the indicated amount. 
    /// </summary>
    public class when_withdrawing_cash_from_the_atm_and_the_amount_exceeds_the_available_funds
        : BaseAggregateWithCommandConsumerTestFixture<Account, 
              WithdrawCashCommand, 
              WithdrawCashCommandConsumer>
    {
        private readonly string _accountNumber = string.Empty;
        private const decimal _avaliableFunds = 150.00M;
        private const decimal _withdrawalAmount = 200.00M;

        public when_withdrawing_cash_from_the_atm_and_the_amount_exceeds_the_available_funds()
        {
            _accountNumber = "1234567890";
        }

        public override IEnumerable<IDomainEvent> Given()
        {
            // create the account and seed it with some money:
            yield return new AccountCreatedEvent(_accountNumber, "Joe", "Smith");
            yield return new CashDepositedEvent(_accountNumber, _avaliableFunds);
        }

        public override WithdrawCashCommand When()
        {
            return new WithdrawCashCommand(Aggregate.Id, _accountNumber, _withdrawalAmount);
        }

        [Fact]
        public void it_will_not_publish_an_event_noting_that_the_cash_has_been_withdrawn()
        {
            PublishedEvents.Latest().WillNotBeOfType<CashWithdrawnEvent>();
        }

        [Fact]
        public void it_will_return_a_message_indicating_the_amount_exceeds_the_available_funds()
        {
            CaughtException.WillBeOfType<WithdrawalAmountExceedsAvaliableFundsException>();
        }
    }
}