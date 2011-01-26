using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Eventing;
using Halifax.Events;

namespace Halifax.Storage.Events
{
    /// <summary>
    /// In-memory storage of domain events from an aggregate root.
    /// </summary>
    public class InMemoryEventStorage : IEventStorage
    {
        private static readonly object _storageLock = new object();
        private readonly IList<PersistableDomainEvent> _persistedEvents;

        public InMemoryEventStorage()
        {
            if (_persistedEvents == null)
                _persistedEvents = new List<PersistableDomainEvent>();
        }

        #region IEventStorage Members

        public void Save<TEvent>(TEvent domainEvent)
            where TEvent : IDomainEvent
        {
            var pe = new PersistableDomainEvent
                         {
                             EventSourceId = domainEvent.AggregateId,
                             Name = domainEvent.GetType().FullName,
                             Event = domainEvent,
                             Version = domainEvent.Version,
                             Timestamp =
                                 domainEvent.EventDateTime.HasValue ? domainEvent.EventDateTime.Value : DateTime.Now,
                         };

            if (!_persistedEvents.Contains(pe))
                lock (_storageLock)
                    _persistedEvents.Add(pe);
        }

        public ICollection<IDomainEvent> GetHistory(Guid aggregateRootId)
        {
            var retval = new List<IDomainEvent>();

            List<PersistableDomainEvent> peristedEvents = (from persistedEvent in _persistedEvents
                                                           where persistedEvent.EventSourceId == aggregateRootId
                                                           select persistedEvent).ToList();

            if (peristedEvents.Count > 0)
                foreach (PersistableDomainEvent domainEvent in peristedEvents)
                    retval.Add(domainEvent.Event);

            return retval;
        }

        public ICollection<IDomainEvent> GetHistorySinceSnapshot(Guid aggregateRootId)
        {
            //TODO: Need to expire the most recent snapshot 
            // and search for the one that is not expired for aggregate
            // re-hydration.
            var retval = new List<IDomainEvent>();

            List<PersistableDomainEvent> events = (from ev in _persistedEvents
                                                   where ev.EventSourceId == aggregateRootId &&
                                                         ev.Name == typeof (AggregateSnapshotCreatedEvent).Name
                                                   orderby ev.Timestamp ascending
                                                   select ev).ToList();

            if (events.Count > 0)
                foreach (PersistableDomainEvent domainEvent in events)
                    retval.Add(domainEvent.Event);

            return retval;
        }

        public ICollection<IDomainEvent> GetCreationEvents()
        {
            var retval = new List<IDomainEvent>();
            var persistableDomainEvents = new List<PersistableDomainEvent>();

            try
            {
                persistableDomainEvents = (from persistedEvent in _persistedEvents
                                           where persistedEvent.Event.GetType() == typeof (AggregateCreatedEvent)
                                           select persistedEvent).ToList();
            }
            catch
            {
            }

            if (persistableDomainEvents.Count > 0)
                foreach (PersistableDomainEvent domainEvent in persistableDomainEvents)
                    retval.Add(domainEvent.Event);

            return retval;
        }

        #endregion
    }
}