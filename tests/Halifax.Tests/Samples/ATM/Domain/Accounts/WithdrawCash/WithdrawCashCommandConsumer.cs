using Halifax;
using Halifax.Commanding;
using Halifax.Storage.Aggregates;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.WithdrawCash
{
    public class WithdrawCashCommandConsumer : 
        CommandConsumer.For<WithdrawCashCommand>
    {
        private readonly IDomainRepository _repository;

        public WithdrawCashCommandConsumer(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWorkSession session, WithdrawCashCommand command)
        {
            var account = _repository.Find<Account>(command.Id);
            account.WithdrawCash(command);
            session.Accept(account);
        }
    }
}