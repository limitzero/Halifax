using Halifax.Eventing;
using Halifax.Tests.Samples.ATM.ReadModel;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash
{
    public class CashDepositedEventConsumer :
        EventConsumer.For<CashDepositedEvent>
    {
        public void Handle(CashDepositedEvent domainEvent)
        {
            ReadModelDB.RecordTransaction(domainEvent);
        }
    }
}