using System;
using Halifax.Commanding;
using Halifax.Storage.Aggregates;
using Halifax.Eventing;

namespace Halifax.Tests
{
    public class TestEntity : AbstractAggregateRootByConvention
    { }

    public class TestCommand : Command
    {
        public Guid Id { get; set; }
    }

    public class TestEvent : DomainEvent
    {}

    public class TestCommandConsumer :
        CommandConsumer.For<TestCommand>
    {
        private readonly IDomainRepository _repository;

        public TestCommandConsumer(IDomainRepository repository)
        {
            this._repository = repository;
        }

        public override void Execute(IUnitOfWork session, TestCommand command)
        {
            var root = _repository.Find<TestEntity>(command.Id);

            using(ITransactedSession txn = session.BeginTransaction(root))
            {
                // call some methods on the aggregate...
            }
            
            // session.Accept(root);
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