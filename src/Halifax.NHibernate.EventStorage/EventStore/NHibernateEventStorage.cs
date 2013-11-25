using System;
using System.Collections.Generic;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Configuration.Impl.Serialization;
using Halifax.Events;
using Halifax.Internals.Events;
using Halifax.NHibernate.Entities;
using NHibernate.Criterion;

namespace Halifax.NHibernate.EventStore
{
    public class NHibernateEventStorage : IEventStorage
    {
    	private readonly INHibernateEventStoreSessionFactory event_store_session_factory;
    	private readonly ISerializationProvider serialization_provider;

        public NHibernateEventStorage(
            INHibernateEventStoreSessionFactory eventStoreSessionFactory,
            ISerializationProvider serializationProvider)
        {
        	event_store_session_factory = eventStoreSessionFactory;
        	serialization_provider = serializationProvider;
        }

        public void Save<TEvent>(TEvent @event) where TEvent : Event
        {
            var data = serialization_provider.Serialize(@event);

        	var event_to_store = StoredEvent.CreateFrom(@event);
        	event_to_store.Data = data;

			using(var session = this.event_store_session_factory.Factory.OpenSession())
			using (var txn = session.BeginTransaction())
            {
                try
                {
					session.Save(event_to_store);
                    txn.Commit();
                }
                catch (Exception e)
                {
                    txn.Rollback();
                    throw e;
                }
            }
        }

        public ICollection<Event> GetHistory(Guid aggregateRootId)
        {
            var results = new List<Event>();

			using (var session = this.event_store_session_factory.Factory.OpenSession())
			{
				var criteria = DetachedCriteria.For<StoredEvent>()
					.Add(Expression.Eq("EventSourceId", aggregateRootId));

				var persistedEvents =
					criteria.GetExecutableCriteria(session).List<StoredEvent>();

				if (persistedEvents.Count > 0)
					foreach (var persistedEvent in persistedEvents)
					{
						var @event = serialization_provider.Deserialize(persistedEvent.Data);
						results.Add(@event as Event);
					}
			}

        	return results;
        }

        public ICollection<Event> GetHistorySinceSnapshot(Guid aggregateRootId)
        {
            var results = new List<Event>();

			using (var session = this.event_store_session_factory.Factory.OpenSession())
			{
				var criteria = DetachedCriteria.For<StoredEvent>()
					.Add(Expression.Eq("Name", typeof (AggregateSnapshotCreatedEvent).Name));
				criteria.AddOrder(Order.Asc("At"));

				var sinceSnapshot =
					criteria.GetExecutableCriteria(session).List<StoredEvent>();

				if (sinceSnapshot.Count > 0)
					foreach (var persistableDomainEvent in sinceSnapshot)
					{
						var @event = serialization_provider.Deserialize(persistableDomainEvent.Data);
						results.Add(@event as Event);
					}
			}

        	return results;
        }

        public ICollection<Event> GetCreationEvents()
        {
            var results = new List<Event>();

			using (var session = this.event_store_session_factory.Factory.OpenSession())
			{
				var criteria = DetachedCriteria.For<StoredEvent>()
					.Add(Expression.Eq("Name", typeof (AggregateCreatedEvent).Name));

				var creationEvents =
					criteria.GetExecutableCriteria(session).List<StoredEvent>();

				if (creationEvents.Count > 0)
					foreach (var persistableDomainEvent in creationEvents)
					{
						var @event = serialization_provider.Deserialize<AggregateCreatedEvent>(persistableDomainEvent.Data);
						results.Add(@event);
					}

			}

        	return results;
        }
    }
}