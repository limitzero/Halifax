using System;
using System.Collections.Generic;
using System.Linq;
using Axiom.Commanding;
using Axiom.Eventing;
using Axiom.Internals.Dispatchers;
using Axiom.Internals.Reflection;
using Axiom.Storage.Aggregates;
using Axiom.Storage.Events;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Axiom.Testing
{
    /// <summary>
    /// Base class for testing commands with the aggregate root and corresponding event handlers.
    /// </summary>
    /// <typeparam name="TAggregate">Type of the aggregate root to respond to the command</typeparam>
    /// <typeparam name="TCommand">Type of the command being issued to the aggregate root entity.</typeparam>
    [Obsolete]
    public abstract class AggregateWithCommandTestFixture<TAggregate, TCommand>
        where TAggregate : AbstractAggregateRoot, new()
        where TCommand : Command
    {
        private IWindsorContainer _container;
        private IStartableCommandBus _commandBus;
        private IStartableEventBus _eventBus;

        public IWindsorContainer Container { get; private set; }

        /// <summary>
        /// The current aggregate root under test.
        /// </summary>
        protected TAggregate Aggregate;

        /// <summary>
        /// (Read-Only). The currently caught exception as captured for 
        /// sending the command to the domain for state change.
        /// </summary>
        protected TheCaughtException CaughtException { get; private set; }

        protected ThePublishedEvents PublishedEvents { get; private set; }

        protected AggregateWithCommandTestFixture()
        {
            InitializeInfrastructure();
            ExecuteContext();
        }

        ~AggregateWithCommandTestFixture()
        {
            if (_commandBus.IsRunning)
                _commandBus.Stop();

            if (_eventBus != null)
                if (_eventBus.IsRunning)
                    _eventBus.Stop();

            _container.Dispose();
        }

        /// <summary>
        /// This will set up the initial domain events to be issued 
        /// against the domain aggregate in order to prepare 
        /// it for accepting the current comand from <see cref="When"/>
        /// case.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IDomainEvent> Given()
        {
            return new List<IDomainEvent>();
        }

        /// <summary>
        /// This will intialize the context for changing the domain 
        /// aggregate via the command {TCommand}. 
        /// </summary>
        public abstract TCommand When();

        public virtual void Finally()
        {
        }

        private void ExecuteContext()
        {
            CaughtException =
                new TheCaughtException(new NoExceptionWasCaughtButOneWasExpectedException());

            // prepare the aggregate:
            PrepareAggregate();

            // create the command:
            var command = When();

            // issue the command against the aggreate:
            try
            {
                _commandBus.Send(command);
            }
            catch (Exception e)
            {
                CaughtException = new TheCaughtException(e);
            }
            finally
            {
                Finally();
            }

            Container = _container;
            PublishedEvents = new ThePublishedEvents(_container, command);
        }

        private void PrepareAggregate()
        {
            var changes = Given();
            if (changes.Count() == 0) return;

            Aggregate = _container.Resolve<IDomainRepository>().Create <TAggregate>();

            Aggregate.LoadFromHistory(Given());
        }

        private void InitializeInfrastructure()
        {
            _container = new WindsorContainer();

            _container.Register(Component.For<TAggregate>()
                                  .ImplementedBy<TAggregate>());

            _container.Register(Component.For<IReflection>()
                                    .ImplementedBy<DefaultReflection>());

            _container.Register(Component.For<IStartableCommandBus>()
                .ImplementedBy<CommandBus>());

            _container.Register(Component.For<IStartableEventBus>()
                .ImplementedBy<EventBus>());

            _container.Register(Component.For<ICommandMessageDispatcher>()
                                   .ImplementedBy<CommandMessageDispatcher>());

            _container.Register(Component.For<IEventMessageDispatcher>()
                                   .ImplementedBy<EventMessageDispatcher>());

            // set the storage for the domain events (in-memory)
            _container.Register(Component.For<IEventStorage>()
                                   .ImplementedBy<InMemoryEventStorage>()
                                   .LifeStyle.Singleton);

            // register all command handlers (default behavior):
            _container.Register(AllTypes.FromAssembly(typeof(TAggregate).Assembly)
                                    .BasedOn(typeof(CommandHandler<>)).WithService.Base());


            // set the storage for the domain aggregates (in-memory for testing):
            _container.Kernel.AddComponent(typeof(IDomainRepository).Name,
                   typeof(IDomainRepository),
                   typeof(DomainRepository));

            // start the commading bus for distributing messages to the command handlers:
            _commandBus = _container.Resolve<IStartableCommandBus>();
            _commandBus.Start();

            // start the eventing bus for distributing messages to the event handlers:
            _eventBus = _container.Resolve<IStartableEventBus>();
            _eventBus.Start();
        }
    }
}