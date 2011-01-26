using System;
using Halifax.Testing;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.ReadModel;
using Xunit;

namespace Halifax.Tests.Samples.ATM.Tests
{
    public class when_a_cash_deposit_is_made_on_the_account_and_request_is_made_to_view_the_balance
        : BaseEventConsumerTestFixture<CashDepositedEvent>
    {
        private const decimal _depositAmount = 150.00M;
        private Guid _accountId = Guid.NewGuid();

        public override void Given()
        {
            ReadModelDB.Refresh();
            RegisterEventConsumerOf<CashDepositedEventConsumer>();
        }

        public override CashDepositedEvent When()
        {
            return new CashDepositedEvent(_accountId.ToString(), _depositAmount);
        }

        [Fact]
        public void it_will_display_the_correct_balance_for_the_account_from_the_read_model()
        {
            PublishedEvents.Latest().WillBeOfType<CashDepositedEvent>();
            Assert.Equal(_depositAmount, ReadModelDB.GetAccountBalance(_accountId).CurrentBalance);
        }
    }
}