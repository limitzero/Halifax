namespace Halifax.Eventing
{
    public interface ISnapshotable
    {
        /// <summary>
        /// (Read-Write). User defined criteria for evaluating when 
        /// the number of 
        /// </summary>
        //Func<bool> IsSnapshotPeriodElapsed { get; set; }
        /// <summary>
        /// (Read-Write).  The current count of events that will be recorded 
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