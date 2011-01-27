using System.Collections.Generic;

namespace Halifax.Eventing
{
    /// <summary>
    /// Contract for all entities that can push/pull/record events that pertain to state changes.
    /// </summary>
    public interface IEventProvider
    {
        /// <summary>
        /// This will clear the current set of changes made to the entity.
        /// </summary>
        void Clear();

        /// <summary>
        /// This will record a change that has been made to the entity.
        /// </summary>
        /// <param name="domainEvent"></param>
        void Record(IDomainEvent domainEvent);

        /// <summary>
        /// This will get the current series of changes that have been made to the entity.
        /// </summary>
        /// <returns></returns>
        ICollection<IDomainEvent> GetChanges();

        /// <summary>
        /// This will reconstitute an object from the events that were previously 
        /// recorded by applying then in order from first to last.
        /// </summary>
        /// <param name="domainEvents">Collection of <seealso cref="IDomainEvent">domain events</seealso>to apply to the entity.</param>
        void LoadFromHistory(IEnumerable<IDomainEvent> domainEvents);
    }
}