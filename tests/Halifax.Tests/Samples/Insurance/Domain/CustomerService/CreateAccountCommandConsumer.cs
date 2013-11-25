using System;
using Halifax.Commands;
using Halifax.Domain;
using Halifax.Tests.Samples.Insurance.Domain.Marketing;

namespace Halifax.Tests.Samples.Insurance.Domain.CustomerService
{
	public class CreateAccountCommandConsumer :
		CommandConsumer.For<CreateAccountCommand>
	{
		private readonly IAggregateRootRepository repository;

		public CreateAccountCommandConsumer(IAggregateRootRepository repository)
		{
			this.repository = repository;
		}

		public override AggregateRoot Execute(CreateAccountCommand command)
		{
			var account = this.repository.Get<Insurance.Domain.Account>(CombGuid.NewGuid());
			account.Create(command.Agent, command.PolicyHolder);
			return account;
		}
	}
}