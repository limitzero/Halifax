using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Domain;
using Halifax.Events;
using Halifax.Events.Module;
using Halifax.Internals.Dispatchers;

namespace Halifax.Configuration.Impl.Eventing.Impl
{
    /// <summary>
    /// Event bus that processes the events from the domain entities
    /// in a synchronous fashion.
    /// </summary>
    public class InProcessEventBus : IEventBus
    {
        private readonly IEventMessageDispatcher dispatcher;
        private readonly IContainer container;

        public InProcessEventBus(
            IContainer container,
            IEventMessageDispatcher dispatcher)
        {
            this.container = container;
            this.dispatcher = dispatcher;
        }

        #region IStartableEventBus Members

        public event EventHandler<EventBusStartPublishMessageEventArgs> EventBusStartMessagePublishEvent;
        public event EventHandler<EventBusCompletedPublishMessageEventArgs> EventBusCompletedMessagePublishEvent;

        public bool IsRunning { get; private set; }

        public object GetComponent(Type component)
        {
            object retval = container.Resolve(component);
            return retval;
        }

        public TComponent GetComponent<TComponent>()
        {
            var retval = container.Resolve<TComponent>();
            return retval;
        }

        public virtual void Publish(AggregateRoot root)
        {
            var changes = new List<Event>(root.GetChanges());
            Publish(changes.ToArray());
        }

        public virtual void Publish<TEVENT>(params TEVENT[] domainEvents)
            where TEVENT : Event
        {
            foreach (TEVENT domainEvent in domainEvents)
            {
                PublishInternal(domainEvent);
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

        private void PublishInternal(Event @event)
        {
            OnStartPublish(@event);

            // send the event to the event handler to persist fire 
            // other events and/or update read model:
            dispatcher.Dispatch(@event);

            OnCompletePublish(@event);
        }

        private void OnCompletePublish(Event @event)
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
                catch
                {
                	throw;
                }
            }
        }

        private void OnStartPublish(Event @event)
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
                catch
                {
                	throw;
                }
            }
        }

        private ICollection<AbstractEventBusModule> FindAllModules()
        {
            var retval = new List<AbstractEventBusModule>();

            try
            {
                IEnumerable<AbstractEventBusModule> modules = container.ResolveAll<AbstractEventBusModule>();
                if (modules.Count() == 0) return retval;
                retval = new List<AbstractEventBusModule>(modules);
            }
            catch
            {
            	throw;
            }

            return retval;
        }
    }
}