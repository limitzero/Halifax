using System;

namespace Halifax.Eventing
{
    public class EventBusCompletedPublishMessageEventArgs : EventArgs
    {
        public EventBusCompletedPublishMessageEventArgs(IDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public IDomainEvent DomainEvent { get; set; }
    }
}