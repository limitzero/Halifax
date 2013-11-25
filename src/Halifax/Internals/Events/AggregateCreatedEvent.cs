namespace Halifax.Internals.Events
{
    /// <summary>
    /// Event that is triggered when an aggregate root
    /// or central point for recording domain consistency
    /// is created for recording the changes to internal 
    /// state as a result of a series of command(s).
    /// </summary>
    public class AggregateCreatedEvent : LifecycleEvent
    {
        public string Aggregate { get; set; }
    }
}