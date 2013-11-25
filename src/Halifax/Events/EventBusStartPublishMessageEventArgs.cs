using System;

namespace Halifax.Events
{
    public class EventBusStartPublishMessageEventArgs : EventArgs
    {
        public EventBusStartPublishMessageEventArgs(Event domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public Event DomainEvent { get; set; }
    }
}