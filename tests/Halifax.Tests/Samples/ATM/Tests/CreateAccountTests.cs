using Halifax.Testing;
using Halifax.Tests.Samples.ATM.Domain.Accounts;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Xunit;

namespace Halifax.Tests.Samples.ATM.Tests
{
    public class when_an_account_is_created_in_the_bank_for_the_customer_and_the_complete_information_is_provided
        : BaseAggregateWithCommandConsumerTestFixture<Account, 
              CreateAccountCommand,
              CreateAccountCommandConsumer>
    {
        private const string _firstname = "Joe";
        private const string _lastname = "Shmoe";
        private const string _teller = "Chuck Knorris";
        private const decimal _initialAmount = 100.00M;

        public override CreateAccountCommand When()
        {
            // the complete information is supplied for creating the account:
            return new CreateAccountCommand(_teller, _firstname, _lastname, _initialAmount);
        }

        [Fact]
        public void it_will_publish_the_event_noting_that_the_account_was_created()
        {
           PublishedEvents.Latest().WillBeOfType<AccountCreatedEvent>();
        }

        [Fact]
        public void it_will_have_the_correct_bank_teller_attached_to_the_account()
        {
            PublishedEvents.Latest<AccountCreatedEvent>().Teller.WillBe(_teller);
        }

        [Fact]
        public void it_will_have_the_correct_custoner_attached_to_the_account()
        {
            PublishedEvents.Latest<AccountCreatedEvent>().FirstName.WillBe(_firstname);
            PublishedEvents.Latest<AccountCreatedEvent>().LastName.WillBe(_lastname);
            PublishedEvents.Latest<AccountCreatedEvent>().InitialAmount.WillBe(_initialAmount);
        }

    }
}