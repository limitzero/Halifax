namespace Halifax.Events
{
    /// <summary>
    /// Event that is created when the level of internal 
    /// events for an entity reaches a certain threshhold.
    /// All entities can be reconsituted to the current 
    /// state from this marker.
    /// </summary>
    public class EntitySnapshotCreatedEvent : LifecycleDomainEvent
    {
        public AbstractAggregateRoot Root { get; set; }
    }
}