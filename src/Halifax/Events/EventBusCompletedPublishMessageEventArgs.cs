using System;

namespace Halifax.Events
{
    public class EventBusCompletedPublishMessageEventArgs : EventArgs
    {
        public EventBusCompletedPublishMessageEventArgs(Event domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public Event DomainEvent { get; set; }
    }
}