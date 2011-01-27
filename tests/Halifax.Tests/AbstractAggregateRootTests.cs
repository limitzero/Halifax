using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Halifax.Bus.Eventing;
using Halifax.Commanding;
using Halifax.Eventing;
using Halifax.Storage.Aggregates;
using Halifax.Storage.Events;
using Xunit;

namespace Halifax.Tests
{
    public class AbstractAggregateRootTests
    {
        private readonly IWindsorContainer _container;
        private readonly IStartableEventBus _eventBus;
        public static ManualResetEvent _wait;
        public static bool _isCreatedEventFired;
        public static bool _isEventHandlerInvoked;
        public static int _convertedEventCount;
        private readonly SampleEntity _root;


        public AbstractAggregateRootTests()
        {
            _container = IoC.BuildContainer();
            _container.Register(Component.For<SampleEntity>().ImplementedBy<SampleEntity>());

            var repository = _container.Resolve<IDomainRepository>();
            _root = repository.Create<SampleEntity>();

            _wait = new ManualResetEvent(false);
        }

        ~AbstractAggregateRootTests()
        {
            if (_container != null)
                _container.Dispose();
        }

        [Fact]
        public void can_register_state_change_when_valid_command_is_applied()
        {
            _root.CreateEntity(new CreateEntityCommand {Name = "tests"});

            var theEvent = (from change in _root.GetChanges()
                            where change.GetType() == typeof(EntityCreatedEvent)
                            select change).FirstOrDefault();

            Assert.IsType(typeof(EntityCreatedEvent), theEvent);
        }

        [Fact]
        public void can_invoke_event_convertor_for_old_event_when_newer_event_is_present_and_it_is_loaded_from_history()
        {
            // create some event history on the aggregate:
            var events = new List<IDomainEvent>();

            for (int i = 0; i < 10; i++)
            {
                events.Add( new EntityCreatedEvent());
            }
            
            _root.LoadFromHistory(events);
            Assert.Equal(10, _convertedEventCount);
        }

        [Serializable]
        public class CreateEntityCommand : Command
        {
            public string Name { get; set; }
        }

        public class EntityCreatedEvent : DomainEvent
        {
            public string Name { get; set; }
        }

        public class EntityCreatedEvent2 : EntityCreatedEvent
        {
            public string Data { get; set; }
        }

        public class EntityCreatedEventHandler :
            EventConsumer.For<EntityCreatedEvent>
        {
            private readonly IEventStorage _storage;

            public EntityCreatedEventHandler(IEventStorage storage)
            {
                _storage = storage;
            }

            public void Handle(EntityCreatedEvent domainEvent)
            {
                // send the message to storage:
                _storage.Save(domainEvent);

                _isEventHandlerInvoked = true;
                _wait.Set();
            }
        }

        public class SampleEntity : AbstractAggregateRootByConvention
            //AbstractAggregateRoot
        {
            #region -- local state --
            private string _name = string.Empty;
            #endregion

            public override void RegisterEvents()
            {
                //RegisterEvent<EntityCreatedEvent>(OnEntityCreatedEvent);
                //RegisterEvent<EntityCreatedEvent2>(OnEntityCreatedEvent2);
            }

            public override void RegisterEventConvertors()
            {
                RegisterConvertor<EntityCreatedEvent, EntityCreatedEvent2>();
            }

            public void CreateEntity(CreateEntityCommand command)
            {
                this.Id = Guid.NewGuid();
                var ev = new EntityCreatedEvent() { Name = command.Name };
                ApplyEvent(ev);
            }

            private void OnEntityCreatedEvent(EntityCreatedEvent domainEvent)
            {
                _isCreatedEventFired = true;

                // update the state:
                _name = domainEvent.Name;
            }

            // here is the aggregate event handler that will be called 
            // for upgrading the event:
            private void Apply(EntityCreatedEvent2 domainEvent)
            {
                _convertedEventCount++;

                // can do this since they are essentially the same:
                this.OnEntityCreatedEvent(domainEvent);
            }



        }
    }
}