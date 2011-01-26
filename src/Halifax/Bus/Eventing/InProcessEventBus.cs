using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Halifax.Eventing;
using Halifax.Eventing.Module;
using Halifax.Storage.Events;

namespace Halifax.Bus.Eventing
{
    /// <summary>
    /// Event bus that processes the events from the domain entities
    /// in a synchronous fashion.
    /// </summary>
    public class InProcessEventBus : IStartableEventBus
    {
        private readonly IEventMessageDispatcher _dispatcher;
        private readonly IEventStorage _eventStorage;
        private readonly IKernel _kernel;

        public InProcessEventBus(
            IKernel kernel,
            IEventStorage eventStorage,
            IEventMessageDispatcher dispatcher)
        {
            _kernel = kernel;
            _eventStorage = eventStorage;
            _dispatcher = dispatcher;
        }

        #region IStartableEventBus Members

        public event EventHandler<EventBusStartPublishMessageEventArgs> EventBusStartMessagePublishEvent;
        public event EventHandler<EventBusCompletedPublishMessageEventArgs> EventBusCompletedMessagePublishEvent;

        public bool IsRunning { get; private set; }

        public object GetComponent(Type component)
        {
            object retval = _kernel.Resolve(component);
            return retval;
        }

        public TComponent GetComponent<TComponent>()
        {
            var retval = _kernel.Resolve<TComponent>();
            return retval;
        }

        public void Publish(AbstractAggregateRoot root)
        {
            var changes = new List<IDomainEvent>(root.GetChanges());
            Publish(changes.ToArray());
        }

        public void Publish<TEVENT>(params TEVENT[] domainEvents)
            where TEVENT : class, IDomainEvent
        {
            // store the events to the event store via the event handlers:
            foreach (TEVENT domainEvent in domainEvents)
            {
                PublishInternal(domainEvent);
                //var theEvent = domainEvent;
                //ThreadPool.QueueUserWorkItem( w => this.PublishInternal(theEvent));
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            if (IsRunning) return;

            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        #endregion

        private void PublishInternal(IDomainEvent @event)
        {
            OnStartPublish(@event);

            // save the event to storage:
            _eventStorage.Save(@event);

            // send the event to the event handler to persist fire 
            // other events and/or update read model:
            _dispatcher.Dispatch(@event);

            OnCompletePublish(@event);
        }

        private void OnCompletePublish(IDomainEvent @event)
        {
            ICollection<AbstractEventBusModule> modules = FindAllModules();
            if (modules.Count == 0) return;

            foreach (AbstractEventBusModule module in modules)
            {
                try
                {
                    var ev = new EventBusCompletedPublishMessageEventArgs(@event);
                    module.OnEventBusCompletedMessagePublishing(ev);
                }
                catch (Exception e)
                {
                }
            }
        }

        private void OnStartPublish(IDomainEvent @event)
        {
            ICollection<AbstractEventBusModule> modules = FindAllModules();
            if (modules.Count == 0) return;

            foreach (AbstractEventBusModule module in modules)
            {
                try
                {
                    var ev = new EventBusStartPublishMessageEventArgs(@event);
                    module.OnEventBusStartMessagePublishing(ev);
                }
                catch (Exception e)
                {
                }
            }
        }

        private ICollection<AbstractEventBusModule> FindAllModules()
        {
            var retval = new List<AbstractEventBusModule>();

            try
            {
                AbstractEventBusModule[] eventBusModules = _kernel.ResolveAll<AbstractEventBusModule>();
                if (eventBusModules.Length == 0) return retval;
                retval = new List<AbstractEventBusModule>(eventBusModules);
            }
            catch (Exception e)
            {
            }

            return retval;
        }
    }
}