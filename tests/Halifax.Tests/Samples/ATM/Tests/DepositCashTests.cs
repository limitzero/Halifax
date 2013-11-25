using System.Collections.Generic;
using Halifax.Events;
using Halifax.Testing;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Machine.Specifications;

namespace Halifax.Tests.Samples.ATM.Tests
{
    /// <summary>
    /// UC3: when the customer attempts to make a cash deposit into to the 
    /// account from the atm, then credit the account by  the deposit amount 
    /// and update the balance.
    /// </summary>
    public class when_making_a_cash_deposit_into_the_account_from_the_atm :
		BaseAggregateRootTestFixture<ATM.Domain.Accounts.Account, DepositCashCommand>
    {
        private readonly string accountNumber = "1234567890";
        private const decimal avaliableFunds = 150.00M;
        private const decimal depositAmount = 50.00M;

        public override IEnumerable<Event> Given()
        {
            // create the account and seed it with some money:
            yield return new AccountCreated("Joe", "Smith",decimal.Zero);
            yield return new CashDeposited(accountNumber, avaliableFunds);
        }

		public override DepositCashCommand When()
        {
            // deposit the cash into the account:
            return new DepositCashCommand(AggregateRoot.Id, depositAmount);
        }

		It it_will_publish_an_event_noting_that_the_cash_has_been_deposited = () =>
		{
			PublishedEvents.Latest().WillBeOfType<CashDeposited>();
		};

		It it_will_record_the_correct_amount_that_is_made_toward_the_deposit = () =>
		{
			PublishedEvents.Latest<CashDeposited>().DepositAmount.WillBe(depositAmount);
		};

    }
}