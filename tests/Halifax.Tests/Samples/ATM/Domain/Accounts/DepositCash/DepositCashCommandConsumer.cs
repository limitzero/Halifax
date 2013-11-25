using Halifax.Commands;
using Halifax.Domain;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash
{
	public class DepositCashCommandConsumer :
		CommandConsumer.For<DepositCashCommand>
	{
		private readonly IAggregateRootRepository repository;
		
		public DepositCashCommandConsumer(IAggregateRootRepository repository)
		{
			this.repository = repository;
		}

		public override AggregateRoot Execute(DepositCashCommand command)
		{
			var account = repository.Get<Account>(command.Id);
			account.DepositCash(command.AccountNumber, command.DepositAmount);
			return account;
		}
	}
}