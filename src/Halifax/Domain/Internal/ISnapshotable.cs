namespace Halifax.Domain.Internal
{
    /// <summary>
    /// Contract for all entities that can take a snapshot of the current series of events
    /// that pertain to state changes to reduce the amount of information brough back 
    /// when reconsituting the entity from historial data.
    /// </summary>
    public interface ISnapshotable
    {
    	/// <summary>
    	/// (Read-Write). User defined criteria for evaluating when 
    	/// the number of recorded events to observe before taking 
    	/// a snapshot of the aggregate.
    	/// </summary>
		int SnapshotLevel { get; set; }

        /// <summary>
        /// Gets or sets The current count of events that will be recorded 
        /// in the aggregate before submitting to the event store.
        /// </summary>
        int GetRecordedEventsCount();

        /// <summary>
        /// This will manually take a snapshot of the current aggregate
        /// and use the events after this marker as a point to build the 
        /// aggregate state.
        /// </summary>
        void TakeSnapshot();
    }
}