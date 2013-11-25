using System.Collections.Generic;
using Halifax.Events;
using Halifax.Testing;
using Halifax.Tests.Samples.ATM.Domain.Accounts;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.Domain.Accounts.Exceptions;
using Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash;
using Machine.Specifications;

namespace Halifax.Tests.Samples.ATM.Tests
{
    /// <summary>
    /// UC1: when the customer attempts to withdraw cash from the atm and the account 
    /// has enough funds availaible to process the transaction, then debit the account by 
    /// the withdrawal amount and allocate the cash to the customer.
    /// </summary>
    public class when_withdrawing_cash_from_the_atm_and_the_account_has_the_available_funds :
		BaseAggregateRootTestFixture<Account, WithdrawCashCommand>
    {
        private const string _teller = "Chuck Knorris";
        private readonly string accountNumber = "1234567890";
        private const decimal avaliableFunds = 150.00M;
        private const decimal withdrawalAmount = 50.00M;

        public override IEnumerable<Event> Given()
        {
            // create the account and seed it with some money:
            yield return new AccountCreated("Joe", "Smith", 0.0M);
            yield return new CashDeposited(accountNumber, avaliableFunds);
        }

		public override WithdrawCashCommand When()
        {
            // withdraw the cash from the account
            return new WithdrawCashCommand(AggregateRoot.Id, accountNumber,withdrawalAmount);
        }

		It it_will_publish_an_event_noting_that_the_cash_has_been_withdrawn = () =>
		{
			PublishedEvents.Latest().WillBeOfType<CashWithdrawn>();
		};

    }

    /// <summary>
    /// UC2: When the customer is attempting to withdraw an amount that exceeds the 
    /// current balance, indicate to the customer that the withdrawal can not be 
    /// completed on the account for the indicated amount. 
    /// </summary>
    public class when_withdrawing_cash_from_the_atm_and_the_amount_exceeds_the_available_funds
		: BaseAggregateRootTestFixture<Account, WithdrawCashCommand>
    {
        private readonly string accountNumber = string.Empty;
        private const decimal avaliableFunds = 150.00M;
        private const decimal withdrawalAmount = 200.00M;

        public when_withdrawing_cash_from_the_atm_and_the_amount_exceeds_the_available_funds()
        {
            accountNumber = "1234567890";
        }

        public override IEnumerable<Event> Given()
        {
            // create the account and seed it with some money:
            yield return new AccountCreated("Joe", "Smith", decimal.Zero);
            yield return new CashDeposited(accountNumber, avaliableFunds);
        }

		public override WithdrawCashCommand When()
        {
            return new WithdrawCashCommand(AggregateRoot.Id, accountNumber, withdrawalAmount);
        }

		It it_will_not_publish_an_event_noting_that_the_cash_has_been_withdrawn = () =>
		{
			PublishedEvents.Latest().WillNotBeOfType<CashWithdrawn>();
		};

		It it_will_return_a_message_indicating_the_amount_exceeds_the_available_funds = () =>
		{
			CaughtException.WillBeOfType<WithdrawalAmountExceedsAvaliableFundsException>();
		};
    }
}