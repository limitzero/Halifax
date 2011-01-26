using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Halifax.Bus.Commanding;
using Halifax.Bus.Eventing;
using Halifax.Commanding;
using Halifax.Eventing;
using Halifax.Events;
using Halifax.Exceptions;
using Halifax.Storage.Events;
using Halifax.Storage.Internals.Dispatchers;
using Halifax.Storage.Internals.Reflection;
using Halifax.Storage.Internals.Serialization;

namespace Halifax.Testing
{
    /// <summary>
    /// Test fixture for invoking an event consumer or series of event consumers for a specific event message
    /// </summary>
    /// <typeparam name="TEvent">Current event to apply to the system to invoke the consumer(s)</typeparam>
    public abstract class BaseEventConsumerTestFixture<TEvent>
        where TEvent : DomainEvent
    {
        private IStartableCommandBus _commandBus;
        private IWindsorContainer _container;
        private IStartableEventBus _eventBus;

        protected BaseEventConsumerTestFixture()
        {
            InitializeContext();
            ExecuteContext();
        }

        /// <summary>
        /// (Read-Only). The currently caught exception as captured for 
        /// sending the evemt to the event consumer.
        /// </summary>
        public TheCaughtException CaughtException { get; set; }

        /// <summary>
        /// (Read-Only). The set of stored events that are published
        /// out by the event bus.
        /// </summary>
        public ThePublishedEvents PublishedEvents { get; private set; }

        /// <summary>
        /// This will set up the test fixture with the initial event consumers
        /// as mentioned in <seealso cref="RegisterEventConsumerOf{TCONSUMER}<>"/>
        /// </summary>
        public virtual void Given()
        {
        }

        /// <summary>
        /// This will return the instance of a collaborator registered
        /// for interaction with the event consumer.
        /// </summary>
        /// <typeparam name="TCOLLABORATOR"></typeparam>
        /// <returns></returns>
        public TCOLLABORATOR ResolveCollaborator<TCOLLABORATOR>()
        {
            return _container.Resolve<TCOLLABORATOR>();
        }

        /// <summary>
        /// This will register an additional component for testing with the event consumer.
        /// </summary>
        /// <typeparam name="TCOLLABORATOR"></typeparam>
        public void RegisterCollaborator<TCOLLABORATOR>() where TCOLLABORATOR : class
        {
            _container.Kernel.Register(Component.For<TCOLLABORATOR>()
                                           .ImplementedBy<TCOLLABORATOR>());
        }

        /// <summary>
        /// This will register an additional component for testing with the event consumer.
        /// </summary>
        /// <typeparam name="TCOLLABORATOR_CONTRACT"></typeparam>
        /// <typeparam name="TCOLLABORATOR_SERVICE"></typeparam>
        public void RegisterCollaborator<TCOLLABORATOR_CONTRACT, TCOLLABORATOR_SERVICE>()
            where TCOLLABORATOR_SERVICE : class, TCOLLABORATOR_CONTRACT
        {
            _container.Kernel.Register(Component.For<TCOLLABORATOR_CONTRACT>()
                                           .ImplementedBy<TCOLLABORATOR_SERVICE>());
        }


        /// <summary>
        /// This will register the current set event consumer(s) to process the 
        /// event as defined by current test event.
        /// </summary>
        /// <typeparam name="TCONSUMER"></typeparam>
        public void RegisterEventConsumerOf<TCONSUMER>() where TCONSUMER : EventConsumer.For<TEvent>
        {
            _container.Kernel.Register(Component.For<TCONSUMER>()
                                           .ImplementedBy<TCONSUMER>());
        }

        /// <summary>
        /// This will register the current set of collaborating event consumer(s) to process the 
        /// events as defined by current test.
        /// </summary>
        /// <typeparam name="TCONSUMER"></typeparam>
        public void RegisterCollaboratingEventConsumerOf<TCONSUMER>() where TCONSUMER : class
        {
            _container.Kernel.Register(Component.For<TCONSUMER>()
                                           .ImplementedBy<TCONSUMER>());
        }

        /// <summary>
        /// This will set up the initial domain events to be issued 
        /// against the event consumer(s) to initialize the event 
        /// storage.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IDomainEvent> WithHistoryOf()
        {
            return new List<IDomainEvent>();
        }

        /// <summary>
        /// This will intialize the context for sending the event to the event consumer.
        /// </summary>
        public abstract TEvent When();

        public virtual void Finally()
        {
        }

        private void ExecuteContext()
        {
            Guid id = Guid.NewGuid();

            CaughtException =
                new TheCaughtException(new NoExceptionWasCaughtButOneWasExpectedException());

            Given();

            // create the event:
            TEvent theEvent = When();

            PrepareTheEvent(theEvent);

            try
            {
                // set up any historical events for processing:
                MapHistoricalEventsToParentsAndSend(theEvent);

                // issue the current event against the consumer(s):
                _eventBus.Publish(theEvent);
            }
            catch (Exception e)
            {
                if (typeof(HalifaxException).IsAssignableFrom(e.GetType()))
                    throw e;

                if(e is NotImplementedException)
                    throw e;

                CaughtException = new TheCaughtException(e);
            }
            finally
            {
                Finally();
            }

            PublishedEvents = new ThePublishedEvents(_container);
        }

        private void PrepareTheEvent(IDomainEvent theEvent)
        {
            // must log the creation event for the fake aggregate
            // to correlate all of the subsequent events against.
            Guid aggregateId = Guid.NewGuid();
            var ce = new AggregateCreatedEvent { AggregateId = aggregateId };
            theEvent.AggregateId = aggregateId;
            _container.Resolve<IEventStorage>().Save(ce);
        }

        private void MapHistoricalEventsToParentsAndSend(IDomainEvent parent)
        {
            var history = new List<IDomainEvent>(WithHistoryOf());

            if (history.Count > 0)
            {
                foreach (IDomainEvent item in history)
                {
                    item.AggregateId = parent.AggregateId;
                    _eventBus.Publish(item);
                }
            }
        }


        private void InitializeContext()
        {
            _container = new WindsorContainer();

            _container.Register(Component.For<ISerializationProvider>()
                                    .ImplementedBy<DataContractSerializationProvider>());

            _container.Register(Component.For<IUnitOfWorkSession>()
                                    .ImplementedBy<UnitOfWorkSession>());

            _container.Register(Component.For<IReflection>()
                                    .ImplementedBy<DefaultReflection>());

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

            // start the commading bus for distributing messages to the command handlers:
            _commandBus = _container.Resolve<IStartableCommandBus>();
            _commandBus.Start();

            // start the eventing bus for distributing messages to the event handlers:
            _eventBus = _container.Resolve<IStartableEventBus>();
            _eventBus.Start();

            // start the eventing bus for distributing messages to the event handlers:
            _eventBus = _container.Resolve<IStartableEventBus>();
            _eventBus.Start();
        }

        ~BaseEventConsumerTestFixture()
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
    }
}