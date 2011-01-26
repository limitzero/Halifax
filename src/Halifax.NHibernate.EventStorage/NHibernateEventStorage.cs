using System;
using System.Collections.Generic;
using Halifax.Eventing;
using Halifax.Events;
using Halifax.NHibernate.EventStorage.Entities;
using Halifax.Storage.Events;
using Halifax.Storage.Internals.Serialization;
using NHibernate.Criterion;

namespace Halifax.NHibernate.EventStorage
{
    public class NHibernateEventStorage : IEventStorage
    {
        private readonly IEventStorageSession _eventStorageSession;
        private readonly ISerializationProvider _serializationProvider;

        public NHibernateEventStorage(
            IEventStorageSession eventStorageSession,
            ISerializationProvider serializationProvider)
        {
            _eventStorageSession = eventStorageSession;
            _serializationProvider = serializationProvider;
        }

        public void Save<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            var data = _serializationProvider.SerializeToBytes(domainEvent);

            var pe = new StoredEvent()
            {
                EventSourceId = domainEvent.AggregateId,
                Name = domainEvent.GetType().FullName,
                Event = domainEvent,
                Version = domainEvent.Version,
                Data = data,
                Timestamp = domainEvent.EventDateTime.HasValue == true ?
                    domainEvent.EventDateTime.Value : System.DateTime.Now,
            };

            using (var txn = _eventStorageSession.Session.BeginTransaction())
            {
                try
                {
                    _eventStorageSession.Session.Save(pe);
                    txn.Commit();
                }
                catch (Exception e)
                {
                    txn.Rollback();
                    throw e;
                }
            }
        }

        public ICollection<IDomainEvent> GetHistory(Guid aggregateRootId)
        {
            var results = new List<IDomainEvent>();

            var criteria = DetachedCriteria.For<StoredEvent>()
                .Add(Expression.Eq("EventSourceId", aggregateRootId));

            var persistableDomainEvents =
                criteria.GetExecutableCriteria(_eventStorageSession.Session).List<StoredEvent>();

            if (persistableDomainEvents.Count > 0)
                foreach (var persistableDomainEvent in persistableDomainEvents)
                {
                    var @event = _serializationProvider.Deserialize(persistableDomainEvent.Data);
                    results.Add(@event as IDomainEvent);
                }

            return results;
        }

        public ICollection<IDomainEvent> GetHistorySinceSnapshot(Guid aggregateRootId)
        {
            var results = new List<IDomainEvent>();

            var criteria = DetachedCriteria.For<StoredEvent>()
                .Add(Expression.Eq("Name", typeof(AggregateSnapshotCreatedEvent).Name));
            criteria.AddOrder(Order.Asc("Timestamp"));

            var sinceSnapshot =
                            criteria.GetExecutableCriteria(_eventStorageSession.Session).List<StoredEvent>();

            if (sinceSnapshot.Count > 0)
                foreach (var persistableDomainEvent in sinceSnapshot)
                {
                    var @event = _serializationProvider.Deserialize(persistableDomainEvent.Data);
                    results.Add(@event as IDomainEvent);
                }

            return results;
        }

        public ICollection<IDomainEvent> GetCreationEvents()
        {
            var results = new List<IDomainEvent>();

            var criteria = DetachedCriteria.For<StoredEvent>()
                .Add(Expression.Eq("Name", typeof(AggregateCreatedEvent).Name));

            var creationEvents =
                criteria.GetExecutableCriteria(_eventStorageSession.Session).List<StoredEvent>();

            if (creationEvents.Count > 0)
                foreach (var persistableDomainEvent in creationEvents)
                {
                    var @event = _serializationProvider.Deserialize(persistableDomainEvent.Data);
                    results.Add(@event as IDomainEvent);
                }


            return results;
        }
    }
}