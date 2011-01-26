using Halifax.Eventing;
using Halifax.Tests.Samples.ATM.ReadModel;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash
{
    public class CashWithdrawnEventConsumer : 
        EventConsumer.For<CashWithdrawnEvent>
    {
        public void Handle(CashWithdrawnEvent domainEvent)
        {
            ReadModelDB.RecordTransaction(domainEvent);
        }
    }
}