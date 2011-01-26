using System;
using Halifax.Eventing;

namespace Halifax.Bus.Eventing
{
    public interface IEventBus
    {
        /// <summary>
        /// Event that is triggered when the event bus is starting to publish a message to the external event handler.
        /// </summary>
        event EventHandler<EventBusStartPublishMessageEventArgs> EventBusStartMessagePublishEvent;

        /// <summary>
        /// Event that is triggered when the event bus has completed publishing a message to the external event handler.
        /// </summary>
        event EventHandler<EventBusCompletedPublishMessageEventArgs> EventBusCompletedMessagePublishEvent;

        object GetComponent(Type component);

        TComponent GetComponent<TComponent>();

        /// <summary>
        /// This will publish all of the recent changes to the aggregate root
        /// out to the event handlers for processing.
        /// </summary>
        /// <param name="root"></param>
        void Publish(AbstractAggregateRoot root);

        /// <summary>
        /// This will send the current commmand message to the appropriate 
        /// command handler and return a result to the caller. 
        /// </summary>
        /// <typeparam name="TEVENT">The type of event to be published</typeparam>
        /// <param name="domainEvents">Array of concrete events for publication</param>
        /// <returns></returns>
        void Publish<TEVENT>(params TEVENT[] domainEvents)
            where TEVENT : class, IDomainEvent;
    }
}