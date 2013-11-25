using System;
using Halifax.Commands;
using Halifax.Domain;

namespace Halifax.Tests.Samples.Insurance.Domain.Auto
{
	public class AutoPolicyCommandConsumer :
		CommandConsumer.For<CreateAutoPolicyCommand>
	{
		private readonly IAggregateRootRepository root_repository;

		public AutoPolicyCommandConsumer(IAggregateRootRepository rootRepository)
		{
			root_repository = rootRepository;
		}

		public override AggregateRoot Execute(CreateAutoPolicyCommand command)
		{
			var account = root_repository.Get<Account>(command.AccountNumber);
			account.CreateAutoPolicy(command.Vehicle, command.PrimaryDriver, command.Drivers);
			return account;
		}
	}
}