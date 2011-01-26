using System;

namespace Halifax.Eventing
{
    public class EventBusStartPublishMessageEventArgs : EventArgs
    {
        public EventBusStartPublishMessageEventArgs(IDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public IDomainEvent DomainEvent { get; set; }
    }
}