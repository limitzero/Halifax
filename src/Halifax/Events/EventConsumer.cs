namespace Halifax.Events
{
    /// <summary>
    /// Base class used for all handling events for event message consumers.
    /// </summary>
    public class EventConsumer
    {
		/// <summary>
		/// Marker interface for container
		/// </summary>
		public interface For
		{
			
		}

        /// <summary>
        /// Base class used for distinguishing the type 
        /// of event message to handle.
        /// </summary>
        /// <typeparam name="T">Type of the event to consume</typeparam>
        public interface For<T> : For
            where T : Event
        {
            /// <summary>
            /// This will take the indicated event and apply it to any interested parties.
            /// </summary>
            /// <param name="event">The event to handle</param>
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
            void Handle(T @event);
        }
    }
}