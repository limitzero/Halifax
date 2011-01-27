namespace Halifax.Eventing
{
    /// <summary>
    /// Base class used for all handling events for event message consumers.
    /// </summary>
    public class EventConsumer
    {
        /// <summary>
        /// Base class used for distinguishing the type 
        /// of event message to handle.
        /// </summary>
        /// <typeparam name="TEVENT">Type of the event to consume</typeparam>
        public interface For<TEVENT>
            where TEVENT : class, IDomainEvent
        {
            /// <summary>
            /// This will take the indicated event and apply it to any interested parties.
            /// </summary>
            /// <param name="theEvent">The event to handle</param>
            /// <example>
            /// <![CDATA[
            /// public class PriceHasBeenUpdatedEventHandler
            ///  : EventConsumer.For<PriceHasBeenUpdatedEvent>
            /// {
            ///    public void Handle(PriceHasBeenUpdatedEvent @event)
            ///    {
            ///        // do something with the event...if needed:
            ///    }
            /// }
            /// ]]>
            /// </example>
            void Handle(TEVENT theEvent);
        }
    }
}