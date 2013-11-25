using Halifax.Events;
using Halifax.Read;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash;
using Halifax.Tests.Samples.ATM.ReadModel;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts
{
    public class AccountTransactionsEventsConsumer : 
        EventConsumer.For<CashWithdrawn>, 
        EventConsumer.For<CashDeposited>, 
        EventConsumer.For<AccountCreated>
    {
    	private readonly IReadModelRepository<AccountTransaction> repository;

    	public AccountTransactionsEventsConsumer(IReadModelRepository<ReadModel.AccountTransaction> repository)
		{
			this.repository = repository;
		}

    	public void Handle(CashWithdrawn @event)
    	{
    		return;
			repository.Insert(new AccountTransaction()
			{
				Id = @event.EventSourceId,
				AccountNumber = @event.AccountNumber,
				Amount = @event.WithdrawalAmount * (-1),
				At = @event.At
			});
        }

        public void Handle(CashDeposited @event)
        {
			return;
			repository.Insert(new AccountTransaction()
			{
				Id = @event.EventSourceId,
				AccountNumber = @event.AccountNumber,
				Amount = @event.DepositAmount,
				At = @event.At
			});
        }

        public void Handle(AccountCreated @event)
        {
			return;
        	repository.Insert(new AccountTransaction()
        	                  	{
        	                  		Id = @event.EventSourceId,
        	                  		AccountNumber = @event.AccountNumber,
        	                  		Amount = @event.InitialAmount,
        	                  		At = @event.At
        	                  	});
        }
    }
}