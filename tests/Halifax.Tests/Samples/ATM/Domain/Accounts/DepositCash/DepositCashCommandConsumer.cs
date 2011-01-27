using Halifax;
using Halifax.Commanding;
using Halifax.Storage.Aggregates;
using Halifax.Tests.Samples.ATM.Domain.Accounts;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash
{
    public class DepositCashCommandConsumer : 
        CommandConsumer.For<DepositCashCommand>
    {
        private readonly IDomainRepository _repository;

        public DepositCashCommandConsumer(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWork session, DepositCashCommand command)
        {
            var account = _repository.Find<Account>(command.Id);

            using (ITransactedSession txn = session.BeginTransaction(account))
            {
                account.MakeCashDeposit(command);
            }

        }
    }
}