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

        public override void Execute(IUnitOfWork session, WithdrawCashCommand command)
        {
            var account = _repository.Find<Account>(command.Id);

            using (ITransactedSession txn = session.BeginTransaction(account))
            {
                account.WithdrawCash(command);
            }
        }
    }
}