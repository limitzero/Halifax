using System;
using Halifax.Storage.Aggregates;
using Xunit;
using Rhino.Mocks;

namespace Halifax.Tests.Commanding
{
    public class when_the_command_handler_is_invoked_with_a_command
    {
        private MockRepository _mocks = null;
        private IDomainRepository _repository;
        private TestCommandConsumer _consumer = null;
        private Guid _id = Guid.Empty;
        private IUnitOfWorkSession _session;

        public when_the_command_handler_is_invoked_with_a_command()
        {
            _mocks = new MockRepository();
            _repository = _mocks.DynamicMock<IDomainRepository>();
            _session = _mocks.DynamicMock<IUnitOfWorkSession>();
            _consumer = new TestCommandConsumer(_repository);
            _id = Guid.NewGuid();
        }

        [Fact]
        public void it_will_resolve_the_domain_aggregate_via_its_repository_to_process_the_command_message()
        {
            var command = new TestCommand() { Id = _id};

            using(_mocks.Record())
            {
                Expect.Call(_repository.Find<TestEntity>(command.Id))
                    .Return(new TestEntity())
                    .Message(
                    "The command comsumer should invoke the domain repository to find the aggregate root for capturing events.");

                _session.Accept(new TestEntity());
                LastCall.IgnoreArguments().Repeat.AtLeastOnce()
                    .Message("The command consumer will accept the changes done to the aggregate root.");
            }

            using(_mocks.Playback())
            {
                _consumer.Execute(_session, command);
            }
        }

        
    }


}