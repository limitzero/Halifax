using System.Collections.Generic;
using Halifax.Eventing;
using Halifax.Testing;
using Halifax.Tests.Samples.ATM.Domain.Accounts;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.ReadModel;
using Xunit;

namespace Halifax.Tests.Samples.ATM.Tests
{
    /// <summary>
    /// UC3: when the customer attempts to make a cash deposit into to the 
    /// account from the atm, then credit the account by  the deposit amount 
    /// and update the balance.
    /// </summary>
    public class when_making_a_cash_deposit_into_the_account_from_the_atm :
        BaseAggregateWithCommandConsumerTestFixture<Account, 
            DepositCashCommand, 
            DepositCashCommandConsumer>
    {
        private const string _teller = "Chuck Knorris";
        private readonly string _accountNumber = "1234567890";
        private const decimal _avaliableFunds = 150.00M;
        private const decimal _depositAmount = 50.00M;

        public override void Initially()
        {
            ReadModelDB.Refresh();
            RegisterCollaborator<CashDepositedEventConsumer>(); 
        }

        public override IEnumerable<IDomainEvent> Given()
        {
            // create the account and seed it with some money:
            yield return new AccountCreatedEvent(_teller, "Joe", "Smith");
            yield return new CashDepositedEvent(_accountNumber, _avaliableFunds);
        }

        public override DepositCashCommand When()
        {
            // deposit the cash into the account:
            return new DepositCashCommand(Aggregate.Id, _depositAmount);
        }

        [Fact]
        public void it_will_publish_an_event_noting_that_the_cash_has_been_deposited()
        {
            PublishedEvents.Latest().WillBeOfType<CashDepositedEvent>();
        }

        [Fact]
        public void it_will_record_the_correct_amount_that_is_made_toward_the_deposit()
        {
            PublishedEvents.Latest<CashDepositedEvent>().DepositAmount.WillBe(_depositAmount);
        }

    }
}