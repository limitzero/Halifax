using Halifax.Eventing;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount
{
    /// <summary>
    /// Handler that is created for the event when an account has been created.
    /// </summary>
    public class AccountCreatedEventHandler : 
        EventConsumer.For<AccountCreatedEvent>
    {
        public AccountCreatedEventHandler()
        {
        }

        public void Handle(AccountCreatedEvent domainEvent)
        {
            // do something with the event, if needed
        }
    }
}