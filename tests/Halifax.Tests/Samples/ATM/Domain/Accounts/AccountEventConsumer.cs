using Halifax.Eventing;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash;
using Halifax.Tests.Samples.ATM.ReadModel;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts
{
    public class AccountEventsConsumer : 
        EventConsumer.For<CashWithdrawnEvent>, 
        EventConsumer.For<CashDepositedEvent>, 
        EventConsumer.For<AccountCreatedEvent>
    {
        public void Handle(CashWithdrawnEvent @event)
        {
            ReadModelDB.RecordTransaction(@event);
        }

        public void Handle(CashDepositedEvent @event)
        {
            ReadModelDB.RecordTransaction(@event);
        }

        public void Handle(AccountCreatedEvent @event)
        {
            throw new System.NotImplementedException();
        }
    }
}