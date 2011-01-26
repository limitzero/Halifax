using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Halifax.Bus.Commanding;
using Halifax.Bus.Eventing;
using Halifax.Commanding;
using Halifax.Eventing;
using Halifax.Exceptions;
using Halifax.Storage.Aggregates;
using Halifax.Storage.Events;
using Halifax.Storage.Internals.Dispatchers;
using Halifax.Storage.Internals.Reflection;

namespace Halifax.Testing
{
    /// <summary>
    /// Base class for testing commands with the aggregate root and corresponding event handlers.
    /// </summary>
    /// <typeparam name="TAggregate">Type of the aggregate root to respond to the command</typeparam>
    /// <typeparam name="TCommand">Type of the command being issued to the aggregate root entity.</typeparam>
    /// <typeparam name="TCommandHandler">Type of the command handler used to process the command into the domain.</typeparam>
    public abstract class BaseAggregateWithCommandConsumerTestFixture<TAggregate, TCommand, TCommandHandler>
        where TAggregate : AbstractAggregateRoot, new()
        where TCommand : Command
        where TCommandHandler : CommandConsumer.For<TCommand>
    {
        private IStartableCommandBus _commandBus;
        private IWindsorContainer _container;
        private IStartableEventBus _eventBus;


        /// <summary>
        /// The current aggregate root under test.
        /// </summary>
        protected TAggregate Aggregate;

        protected BaseAggregateWithCommandConsumerTestFixture()
        {
            InitializeInfrastructure();
            ExecuteContext();
        }

        /// <summary>
        /// (Read-Only). The currently caught exception as captured for 
        /// sending the command to the domain for state change.
        /// </summary>
        //protected TheCaughtException CaughtException { get; private set; }
        protected Exception CaughtException { get; set; }

        protected ThePublishedEvents PublishedEvents { get; private set; }

        ~BaseAggregateWithCommandConsumerTestFixture()
        {
            if (_commandBus != null)
                if (_commandBus.IsRunning)
                    _commandBus.Stop();


            if (_eventBus != null)
                if (_eventBus.IsRunning)
                    _eventBus.Stop();

            if (_container != null)
                _container.Dispose();
        }

        /// <summary>
        /// Registers a compoent for interaction with the command 
        /// consumer and aggregate root.
        /// </summary>
        /// <typeparam name="TComponent">The concrete component to register</typeparam>
        protected void RegisterCollaborator<TComponent>() where TComponent : class
        {
            _container.Register(Component.For<TComponent>()
                                    .ImplementedBy<TComponent>());
        }

        /// <summary>
        /// Registers a compoent for interaction with the command 
        /// consumer and aggregate root.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TService"></typeparam>
        protected void RegisterCollaborator<TContract, TService>()
            where TService : class, TContract
        {
            _container.Register(Component.For<TContract>()
                                    .ImplementedBy<TService>());
        }


        /// <summary>
        /// This will send an additional command to the domain 
        /// after the initial command issued in the <seealso cref="When"/>
        /// pre-condition has been sent.
        /// </summary>
        /// <param name="command"></param>
        protected void SendAdditionalCommandOf(Command command)
        {
            try
            {
                _commandBus.Send(command);
            }
            catch (Exception e)
            {
                if (typeof(HalifaxException).IsAssignableFrom(e.GetType()))
                    throw e;

                //CaughtException = new TheCaughtException(e);
                CaughtException = e;
            }
        }

        /// <summary>
        /// This will set up the fixture for any dependencies 
        /// or pre-test data staging.
        /// </summary>
        public virtual void Initially()
        {
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
            CaughtException = new NoExceptionWasCaughtButOneWasExpectedException();
            
            Initially();

            // prepare the aggregate:
            PrepareAggregate();

            // create the command:
            TCommand command = When();

            // issue the command against the aggreate:
            try
            {
                _commandBus.Send(command);
            }
            catch (Exception e)
            {
                if (typeof(HalifaxException).IsAssignableFrom(e.GetType()))
                    throw e;

                //CaughtException = new TheCaughtException(e);
                CaughtException = e;
            }
            finally
            {
                Finally();
            }

            PublishedEvents = new ThePublishedEvents(_container, command);
        }

        private void PrepareAggregate()
        {
            IEnumerable<IDomainEvent> changes = Given();
            if (changes.Count() == 0) return;

            Aggregate = _container.Resolve<IDomainRepository>().Create<TAggregate>();

            Aggregate.LoadFromHistory(Given());

            foreach (var change in changes)
                this._eventBus.Publish(change);
        }

        private void InitializeInfrastructure()
        {
            _container = new WindsorContainer();

            _container.Register(Component.For<TAggregate>()
                                    .ImplementedBy<TAggregate>());

            _container.Register(Component.For<IReflection>()
                                    .ImplementedBy<DefaultReflection>());

            _container.Register(Component.For<IUnitOfWorkSession>()
                                    .ImplementedBy<UnitOfWorkSession>());

            _container.Register(Component.For<IStartableCommandBus>()
                                    .ImplementedBy<InProcessCommandBus>());

            _container.Register(Component.For<IStartableEventBus>()
                                    .ImplementedBy<InProcessEventBus>());

            _container.Register(Component.For<ICommandMessageDispatcher>()
                                    .ImplementedBy<CommandMessageDispatcher>());

            _container.Register(Component.For<IEventMessageDispatcher>()
                                    .ImplementedBy<EventMessageDispatcher>());

            // set the storage for the domain events (in-memory)
            _container.Register(Component.For<IEventStorage>()
                                    .ImplementedBy<InMemoryEventStorage>()
                                    .LifeStyle.Singleton);

            // register all command handlers (default behavior):
            _container.Register(Component.For<CommandConsumer.For<TCommand>>()
                                    .ImplementedBy<TCommandHandler>());

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