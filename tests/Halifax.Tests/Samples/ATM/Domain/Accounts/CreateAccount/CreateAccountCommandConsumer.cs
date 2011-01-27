using Halifax.Commanding;
using Halifax.Storage.Aggregates;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount
{
    public class CreateAccountCommandConsumer
        : CommandConsumer.For<CreateAccountCommand>
    {
        private readonly IDomainRepository _repository;

        public CreateAccountCommandConsumer(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWork session, CreateAccountCommand command)
        {
            var account = _repository.Create<Account>();

            using (ITransactedSession txn = session.BeginTransaction(account))
            {
                account.Create(command);
            }
           
        }
    }
}