using Halifax.Events;
using Halifax.Read;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount
{
    /// <summary>
    /// Handler that is created for the event when an account has been created.
    /// </summary>
    public class AccountCreatedEventHandler : 
        EventConsumer.For<AccountCreated>
    {
    	private readonly IReadModelRepository<ReadModel.Account> repository;

    	public AccountCreatedEventHandler(IReadModelRepository<ReadModel.Account> repository)
        {
        	this.repository = repository;
        }

    	public void Handle(AccountCreated @event)
        {
			this.repository.Insert(new ReadModel.Account()
			                       	{
										Id = @event.EventSourceId,
			                       		AccountNumber =  @event.AccountNumber, 
										Balance = @event.Balance,
										FirstName = @event.FirstName, 
										LastName = @event.LastName
			                       	});
        }
    }
}