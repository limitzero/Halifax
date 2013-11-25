using System;
using Halifax.Configuration.Impl.EventStorage.Impl;
using Halifax.Domain;
using Halifax.Events;

namespace Halifax.NHibernate.Entities
{
    /// <summary>
    /// Created to keep the mapping and the mapped entity in the same 
    /// assembly for consistency.
    /// </summary>
    [Serializable]
    public class StoredEvent 
    {
		public static StoredEvent CreateFrom(Event @event)
		{
			var stored_event = new StoredEvent
			                   	{
			                   		EventSourceId = @event.EventSourceId,
			                   		Event = @event,
			                   		At = @event.At,
			                   		Name = @event.GetType().FullName, 
									Version = @event.Version, 
									Who = @event.Who,
									From = @event.From
			                   	};
			return stored_event;
		}

    	/// <summary>
		/// (Read-Only). Identifier used by the persistant storage mechanism.
		/// </summary>
		public virtual int Id { get; set; }

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
		public virtual DateTime At { get; set; }

		/// <summary>
		/// (Read-Write). The serialized text of the event instance 
		/// used for storage.
		/// </summary>
		public virtual string Data { get; set; }

		/// <summary>
		/// (Read-Write). The current instance of the event used for simple
		/// in-memory querying.
		/// </summary>
		public virtual Event Event { get; set; }

		/// <summary>
		/// (Read-Write). The name of the event that is persisted.
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// (Read-Write). The username on the thread that created the event.
		/// </summary>
		public virtual string Who { get; set; }

		/// <summary>
		/// Gets or sets the entity that emitted the event.
		/// </summary>
		public virtual string From { get; set; }
    }
}