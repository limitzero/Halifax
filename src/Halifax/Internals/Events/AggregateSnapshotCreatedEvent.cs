using Halifax.Domain;

namespace Halifax.Internals.Events
{
    /// <summary>
    /// Event that is created when the level of internal 
    /// events for an aggregate reaches a certain threshhold.
    /// All aggregates can be reconsituted to the current 
    /// state from this marker.
    /// </summary>
    public class AggregateSnapshotCreatedEvent : LifecycleEvent
    {
        public AggregateRoot Root { get; set; }
    }
}