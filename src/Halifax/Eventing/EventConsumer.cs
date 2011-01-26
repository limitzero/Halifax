namespace Halifax.Eventing
{
    /// <summary>
    /// Base class used for all event message consumers.
    /// </summary>
    public class EventConsumer
    {
        #region Nested type: For

        /// <summary>
        /// Base class used for distinguishing the type 
        /// of event message to handle.
        /// </summary>
        /// <typeparam name="TEVENT">Type of the event to consume</typeparam>
        public interface For<TEVENT>
            where TEVENT : class, IDomainEvent
        {
            void Handle(TEVENT theEvent);
        }

        #endregion
    }
}