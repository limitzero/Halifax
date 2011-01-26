using System;

namespace Halifax.Eventing
{
    [Serializable]
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public string Who { get; set; }
        public DateTime? EventDateTime { get; set; }
    }
}