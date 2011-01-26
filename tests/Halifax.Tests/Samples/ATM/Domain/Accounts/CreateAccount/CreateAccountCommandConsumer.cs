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

        public override void Execute(IUnitOfWorkSession session, CreateAccountCommand command)
        {
            var account = _repository.Create<Account>();
            account.Create(command);
            session.Accept(account);
        }
    }
}