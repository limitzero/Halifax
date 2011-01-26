using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Halifax.Bus.Eventing.Async.Endpoints;
using Halifax.Bus.Eventing.Async.Pipeline;
using Halifax.Bus.Eventing.Async.Subscriptions;
using Halifax.Bus.Eventing.Async.Transport;
using Halifax.Eventing;
using Halifax.Eventing.Module;
using Halifax.Storage.Events;

namespace Halifax.Bus.Eventing.Async
{
    /// <summary>
    /// Asynchronous eventing bus that uses MSMQ to 
    /// store messages for retreiving in an offline fashion.
    /// </summary>
    public class AsyncEventBus : IStartableEventBus
    {
        private readonly IEventMessageDispatcher _dispatcher;
        private readonly IEndpointManager _endpointManager;
        private readonly IKernel _kernel;
        private readonly IPipelineManager _pipelineManager;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly ITransport _transport;

        public AsyncEventBus(
            IKernel kernel,
            ITransport transport,
            IPipelineManager pipelineManager,
            IEndpointManager endpointManager,
            ISubscriptionManager subscriptionManager,
            IEventMessageDispatcher dispatcher)
        {
            _kernel = kernel;
            _transport = transport;
            _pipelineManager = pipelineManager;
            _endpointManager = endpointManager;
            _subscriptionManager = subscriptionManager;
            _dispatcher = dispatcher;
        }

        #region IStartableEventBus Members

        public event EventHandler<EventBusStartPublishMessageEventArgs> EventBusStartMessagePublishEvent;
        public event EventHandler<EventBusCompletedPublishMessageEventArgs> EventBusCompletedMessagePublishEvent;

        public bool IsRunning { get; private set; }

        public void Start()
        {
            // start all of the messaging endpoints
            ActivateEndpoints();
            IsRunning = true;
        }

        public void Stop()
        {
            DeactivateEndpoints();
            IsRunning = false;
        }


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
            ICollection<IDomainEvent> changes = root.GetChanges();
            PublishInternal(changes);
        }

        public void Publish<TEvent>(params TEvent[] domainEvents) where TEvent : class, IDomainEvent
        {
            PublishInternal(domainEvents);
        }

        public void Dispose()
        {
            Stop();
        }

        #endregion

        private void PublishInternal(IEnumerable<IDomainEvent> events)
        {
            foreach (IDomainEvent domainEvent in events)
            {
                try
                {
                    OnStartPublish(domainEvent);
                    ISubscription[] subscriptions = _subscriptionManager.GetSubscriptions(domainEvent);
                    _pipelineManager.InvokeForSend(subscriptions, domainEvent, _transport);
                    OnCompletePublish(domainEvent);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private void ActivateEndpoints()
        {
            _endpointManager.EndpointTransportMessageReceivedEvent += OnTransportMessageReceivedEvent;
            _endpointManager.EndpointTransportErrorEvent += OnTransportErrorEvent;
            _endpointManager.Start();
        }

        private void DeactivateEndpoints()
        {
            _endpointManager.Stop();
            _endpointManager.EndpointTransportMessageReceivedEvent -= OnTransportMessageReceivedEvent;
            _endpointManager.EndpointTransportErrorEvent -= OnTransportErrorEvent;
        }

        private void OnTransportErrorEvent(object sender, TransportErrorEventArgs e)
        {
            //TODO: handle this error message and push to separate queue
        }

        private void OnTransportMessageReceivedEvent(object sender, TransportMessageReceivedEventArgs e)
        {
            // save the event to storage and the event handler will update the "read model":
            IDomainEvent message = _pipelineManager.InvokeForReceive(e.Message);
            GetComponent<IEventStorage>().Save(message);

            // send the event to the event handler to persist fire other events and/or update read model:
            _dispatcher.Dispatch(message);
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