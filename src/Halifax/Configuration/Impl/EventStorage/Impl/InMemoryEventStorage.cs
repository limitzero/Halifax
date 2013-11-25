using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Events;
using Halifax.Internals.Events;

namespace Halifax.Configuration.Impl.EventStorage.Impl
{
	/// <summary>
	/// In-memory storage of domain events from an aggregate root.
	/// </summary>
	public class InMemoryEventStorage : IEventStorage
	{
		private static readonly object storage_lock = new object();
		private readonly IList<PersistableDomainEvent> persisted_events;

		public InMemoryEventStorage()
		{
			if (persisted_events == null)
				persisted_events = new List<PersistableDomainEvent>();
		}

		#region IEventStorage Members

		public void Save<TEvent>(TEvent @event)
			where TEvent : Event
		{
			var pe = new PersistableDomainEvent
						 {
							 EventSourceId = @event.EventSourceId,
							 Name = @event.GetType().FullName,
							 Event = @event,
							 Version = @event.Version,
							 Timestamp = @event.At
						 };

			lock (storage_lock)
			{
				persisted_events.Add(pe);
			}
		}

		public ICollection<Event> GetHistory(Guid aggregateRootId)
		{
			var retval = new List<Event>();
			List<PersistableDomainEvent> peristedEvents = new List<PersistableDomainEvent>();

			lock (storage_lock)
			{
				peristedEvents = (from persistedEvent in persisted_events
								  where persistedEvent.EventSourceId == aggregateRootId
								  select persistedEvent).ToList();
			}

			if (peristedEvents.Count > 0)
				foreach (PersistableDomainEvent domainEvent in peristedEvents)
					retval.Add(domainEvent.Event);

			return retval;
		}

		public ICollection<Event> GetHistorySinceSnapshot(Guid aggregateRootId)
		{
			//TODO: Need to expire the most recent snapshot 
			// and search for the one that is not expired for aggregate
			// re-hydration.
			var retval = new List<Event>();
			List<PersistableDomainEvent> events = new List<PersistableDomainEvent>();

			lock (storage_lock)
			{
				events = (from ev in persisted_events
						  where ev.EventSourceId == aggregateRootId &&
								ev.Name == typeof(AggregateSnapshotCreatedEvent).Name
						  orderby ev.Timestamp ascending
						  select ev).ToList();
			}

			if (events.Count > 0)
				foreach (PersistableDomainEvent domainEvent in events)
					retval.Add(domainEvent.Event);

			return retval;
		}

		public ICollection<Event> GetCreationEvents()
		{
			var retval = new List<Event>();
			var persistableDomainEvents = new List<PersistableDomainEvent>();

			lock (storage_lock)
			{
				persistableDomainEvents = (from persistedEvent in persisted_events
										   where persistedEvent.Event.GetType() == typeof(AggregateCreatedEvent)
										   select persistedEvent).ToList();
			}

			if (persistableDomainEvents.Count > 0)
				foreach (PersistableDomainEvent domainEvent in persistableDomainEvents)
					retval.Add(domainEvent.Event);

			return retval;
		}

		#endregion
	}
}