using Halifax.Eventing;

namespace Halifax.Internals.Convertor
{
    /// <summary>
    /// Contract used for converting old events into new events
    /// when taking the messages from the event storage and 
    /// applying them to the domain aggregate.
    /// </summary>
    public interface IEventConvertor
    {
        /// <summary>
        /// Gets the type of the new event that is being rendered through a conversion.
        /// </summary>
        IDomainEvent NewEvent { get; }

        /// <summary>
        /// Gets the type of the old event that is in need of conversion.
        /// </summary>
        IDomainEvent OldEvent { get; }

        /// <summary>
        /// This will convert an event to its newer representation for use in the aggregate.
        /// </summary>
        /// <returns></returns>
        IDomainEvent Convert(IDomainEvent @event);

        /// <summary>
        /// This will register the conversion for the old event into the new event with 
        /// the stipulation that the new event must derive from the old event.
        /// </summary>
        /// <typeparam name="TNEWEVENT"></typeparam>
        /// <typeparam name="TOLDEVENT"></typeparam>
        /// <returns></returns>
        void RegisterConversion<TNEWEVENT, TOLDEVENT>()
            where TOLDEVENT : class, IDomainEvent, new()
            where TNEWEVENT : class, TOLDEVENT, new();
    }
}