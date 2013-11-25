using Halifax.Commands;
using Halifax.Domain;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount
{
    public class CreateAccountCommandConsumer
        : CommandConsumer.For<CreateAccountCommand>
    {
		private readonly IAggregateRootRepository repository;

        public CreateAccountCommandConsumer(IAggregateRootRepository repository)
        {
			this.repository = repository;
        }

        public override AggregateRoot Execute(CreateAccountCommand command)
        {
			var account = repository.Get<Account>(command.Id);
			account.Create(command.FirstName, command.LastName, command.InitialAmount);
        	return account;
        }
    }
}