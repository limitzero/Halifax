using Halifax.Events;
using Halifax.Read;
using Halifax.Tests.Samples.ATM.ReadModel;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash
{
    public class CashWithdrawnEventConsumer : 
        EventConsumer.For<CashWithdrawn>
    {
		private readonly IReadModelRepository<ReadModel.AccountTransaction> repository;

    	public CashWithdrawnEventConsumer(IReadModelRepository<ReadModel.AccountTransaction> repository)
    	{
    		this.repository = repository;
    	}

    	public void Handle(CashWithdrawn @event)
    	{
    		repository.Insert(new AccountTransaction()
    		                  	{
    		                  		Id = @event.EventSourceId,
									AccountNumber = @event.AccountNumber,
    		                  		Amount = @event.WithdrawalAmount* (-1),
    		                  		At = @event.At
    		                  	});

    	}
    }
}