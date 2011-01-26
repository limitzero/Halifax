using System;
using System.Threading;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Halifax.Bus.Commanding;
using Halifax.Commanding;
using Halifax.Configuration;
using Halifax.Configuration.Bootstrapper;
using Halifax.Configuration.Infrastructure;
using Halifax.Eventing;
using Halifax.Storage.Aggregates;
using Rhino.Mocks;
using Xunit;

namespace Halifax.Tests.Commanding
{
    public class CommandBusTests : IDisposable
    {
        public static Command _received_command = null;
        public static DomainEvent _received_event = null;
        public static ManualResetEvent _wait = null;

        private readonly MockRepository _mocks;
        private readonly IKernel _kernel;
        private readonly ICommandMessageDispatcher _dispatcher;
        private readonly InProcessCommandBus _bus = null;

        public CommandBusTests()
        {
            _mocks = new MockRepository();
            _kernel = _mocks.DynamicMock<IKernel>();
            _dispatcher = _mocks.DynamicMock<ICommandMessageDispatcher>();

            _bus = new InProcessCommandBus(_kernel, _dispatcher);

            _wait = new ManualResetEvent(false);
        }

        public void Dispose()
        {
            if(_wait != null)
            {
                _wait.Close();
                _wait = null;
            }
        }

        [Fact]
        public void can_invoke_the_command_dispatcher_to_forward_the_command_message_the_the_appropriate_handler()
        {
            var command = new SampleCommand();
            var handler = new SampleCommandConsumer();

            using (_mocks.Record())
            {
                _dispatcher.Dispatch(command);
                LastCall.Repeat.AtLeastOnce().Message(
                    "The bus should enlist the dispatcher to deliver the message to the handler.");
            }

            using (_mocks.Playback())
            {
                _bus.Send(command);
            }
        }

        [Fact]
        public void can_dispatch_message_to_command_handler_via_the_command_bus_and_the_message_reach_the_handler()
        {
            // use the context object that wraps the container and both command and event bus
            // to send the command to the domain :
            using (var ctx = new HalifaxContext(@"sample.async.config.xml"))
            {
                ctx.Send(new SampleCommand());
                _wait.WaitOne(TimeSpan.FromSeconds(5));
                Assert.Equal(typeof(SampleCommand), _received_command.GetType());
            }
        }

        [Fact]
        public void can_send_command_and_corresponding_event_reach_the_event_consumer()
        {
            // use the context object that wraps the container and both command and event bus
            // to send the command to the domain:
            using (var ctx = new HalifaxContext(@"sample.sync.config.xml"))
            {
                ctx.Send(new SampleDoWorkCommand());
                _wait.WaitOne(TimeSpan.FromSeconds(5));
                Assert.Equal(typeof(SampleDoWorkEvent), _received_event.GetType());
            }
        }
          
        public class SampleCommand : Command
        {
            public Guid Id { get; set; }
        }

        public class SampleDoWorkCommand : Command
        {
            public Guid Id { get; set; }
        }

        public class SampleDoWorkEvent : DomainEvent
        {}

        public class SampleCommandConsumer
            : CommandConsumer.For<SampleCommand>
        {
            public override void Execute(IUnitOfWorkSession session, SampleCommand command)
            {
                _received_command = command;
                _wait.Set();
            }
        }

        public class SampleDoWorkCommandConsumer : 
            CommandConsumer.For<SampleDoWorkCommand>
        {
            private readonly IDomainRepository _repository;

            public SampleDoWorkCommandConsumer(IDomainRepository repository)
            {
                _repository = repository;
            }

            public override void Execute(IUnitOfWorkSession session, SampleDoWorkCommand theCommand)
            {
                var sample = _repository.Create<Sample>();
                sample.DoWork();
                session.Accept(sample);
            }
        }

        public class SampleDoWorkEventConsumer : 
            EventConsumer.For<SampleDoWorkEvent>
        {
            public void Handle(SampleDoWorkEvent theEvent)
            {
                _received_event = theEvent;
                _wait.Set();
            }
        }

        public class Sample : AbstractAggregateRootByConvention
        {
            public void DoWork()
            {
                var e = new SampleDoWorkEvent();
                ApplyEvent(e);
            }

            public void OnSampleDoWorkEvent(SampleDoWorkEvent theEvent)
            {
            }
        }
    }

    public class SampleBootstrapper : AbstractBootstrapper
    {
        public override void Configure()
        {
            Kernel.Register(Component.For<CommandBusTests.SampleCommandConsumer>()
                                .ImplementedBy<CommandBusTests.SampleCommandConsumer>());

            Kernel.Register(Component.For<CommandBusTests.SampleDoWorkEventConsumer>()
                    .ImplementedBy<CommandBusTests.SampleDoWorkEventConsumer>());

            Kernel.Register(Component.For<CommandBusTests.Sample>()
                    .ImplementedBy<CommandBusTests.Sample>());

        }
    }
}