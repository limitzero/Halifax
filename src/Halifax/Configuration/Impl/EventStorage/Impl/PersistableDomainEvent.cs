using System;
using Halifax.Events;

namespace Halifax.Configuration.Impl.EventStorage.Impl
{
    /// <summary>
    /// This is the object that will be persisted to storage 
    /// for recording domain events (in-memory implementation only).
    /// </summary>
    [Serializable]
    public class PersistableDomainEvent
    {
        /// <summary>
        /// (Read-Only). Identifier used by the persistant storage mechanism.
        /// </summary>
        public virtual int Id { get;  set; }

        /// <summary>
        /// (Read-Write). The current identifier of the aggregate that 
        /// this event is mapped to.
        /// </summary>
        public virtual Guid EventSourceId { get; set; }

        /// <summary>
        /// (Read-Write). The current version of the aggregate that this 
        /// is mapped to.
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// (Read-Write). Timestamp for the current event.
        /// </summary>
        public virtual DateTime Timestamp { get; set; }

        /// <summary>
        /// (Read-Write). The serialized byte array of the event instance 
        /// used for storage.
        /// </summary>
        public virtual byte[] Data { get; set; }

        /// <summary>
        /// (Read-Write). The current instance of the event used for simple
        /// in-memory querying.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// (Read-Write). The name of the event that is persisted.
        /// </summary>
        public virtual string Name { get; set; }
    }
}