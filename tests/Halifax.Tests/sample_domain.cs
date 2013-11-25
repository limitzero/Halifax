using System;
using Halifax.Commands;
using Halifax.Domain;
using Halifax.Events;

namespace Halifax.Tests
{
	public class TestEntity : AggregateRoot
	{ }

	public class TestCommand : Command
	{
		public Guid Id { get; set; }
	}

	public class TestEvent : Event
	{ }

	public class TestCommandConsumer :
		CommandConsumer.For<TestCommand>
	{
		private readonly IAggregateRootRepository root_repository;

		public TestCommandConsumer(IAggregateRootRepository rootRepository)
		{
			this.root_repository = rootRepository;
		}

		public override AggregateRoot Execute(TestCommand command)
		{
			var root = root_repository.Get<TestEntity>(command.Id);
			return root;
		}
	}

	public class TestEventConsumer :
		EventConsumer.For<TestEvent>
	{
		public void Handle(TestEvent @event)
		{

		}
	}
}