using System;
using Halifax.Events;
using Halifax.Read;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash
{
    public class CashDepositedEventConsumer :
        EventConsumer.For<CashDeposited>
    {
		private readonly IReadModelRepository<ReadModel.AccountTransaction> repository;

    	public CashDepositedEventConsumer(IReadModelRepository<Samples.ATM.ReadModel.AccountTransaction> repository)
		{
			this.repository = repository;
		}

    	public void Handle(CashDeposited @event)
        {
			repository.Insert(new ReadModel.AccountTransaction()
			                  	{
			                  		Id  = @event.EventSourceId, 
									AccountNumber = @event.AccountNumber,
									Amount = @event.DepositAmount,
									At = @event.At
			                  	});
        }
    }
}