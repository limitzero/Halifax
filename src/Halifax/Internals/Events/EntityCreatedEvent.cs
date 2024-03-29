namespace Halifax.Internals.Events
{
    /// <summary>
    /// Event that is triggered when an entity, other 
    /// than an aggregate is created for recording 
    /// changes to the state as a result of commands
    /// carried out on the aggregate.
    /// </summary>
    public class EntityCreatedEvent : LifecycleEvent
    {
        public string EntityName { get; set; }
    }
}