using Halifax;
using Halifax.Commands;
using Halifax.Domain;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash
{
    public class WithdrawCashCommandConsumer : 
        CommandConsumer.For<WithdrawCashCommand>
    {
        private readonly IAggregateRootRepository root_repository;

        public WithdrawCashCommandConsumer(IAggregateRootRepository rootRepository)
        {
            root_repository = rootRepository;
        }

        public override AggregateRoot Execute(WithdrawCashCommand command)
        {
            var account = root_repository.Get<Account>(command.Id);
			account.WithdrawCash(command.AccountNumber, command.WithdrawalAmount);
        	return account;
        }
    }
}