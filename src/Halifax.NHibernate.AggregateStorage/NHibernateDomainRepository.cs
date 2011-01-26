using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Halifax.Bus.Eventing;
using Halifax.Events;
using Halifax.NHibernate.AggregateStorage.Entities;
using Halifax.Storage.Aggregates;
using Halifax.Storage.Events;
using Halifax.Storage.Internals.Serialization;
using NHibernate.Criterion;

namespace Halifax.NHibernate.AggregateStorage
{
    public class NHibernateDomainRepository : IDomainRepository
    {
        private readonly IKernel _kernel;
        private readonly IAggregateStorageSession _aggregateStorageSession;
        private readonly IStartableEventBus _eventBus;
        private readonly IEventStorage _eventStorage;
        private readonly ISerializationProvider _serializationProvider;

        public NHibernateDomainRepository(
            IKernel kernel,
            IAggregateStorageSession aggregateStorageSession,
            IStartableEventBus eventBus,
            IEventStorage eventStorage,
            ISerializationProvider serializationProvider)
        {
            _kernel = kernel;
            _aggregateStorageSession = aggregateStorageSession;
            _eventBus = eventBus;
            _eventStorage = eventStorage;
            _serializationProvider = serializationProvider;
        }

        public TEntity Create<TEntity>() where TEntity : AbstractAggregateRoot, new()
        {
            var entity = _kernel.Resolve<TEntity>();

            var ev = new AggregateCreatedEvent()
            {
                AggregateId = entity.Id,
                Aggregate = entity.GetType().FullName,
                EventDateTime = System.DateTime.Now
            };

            _eventStorage.Save(ev);

            return entity;
        }

        public TEntity Find<TEntity>(Guid Id) where TEntity : AbstractAggregateRoot
        {
            var entity = default(TEntity);
            StoredDomainAggregate aggregate;

            var events = _eventStorage.GetCreationEvents();

            var ev = (from e in events
                      let ce = e as AggregateCreatedEvent
                      where ce.AggregateId == Id
                      select ce).FirstOrDefault();

            if (ev == null) return default(TEntity);

            try
            {
                aggregate = _aggregateStorageSession.Session.Get<StoredDomainAggregate>(ev.AggregateId);
            }
            catch (Exception e)
            {
                throw e;
            }

            if (aggregate != null)
            {
                entity = _serializationProvider.Deserialize<TEntity>(aggregate.Data);
                //entity.SetEventPublisher(_eventBus);
            }

            return entity;
        }

        public ICollection<TEntity> GetAll<TEntity>() where TEntity : AbstractAggregateRoot
        {
            var entities = new List<TEntity>();
            IList<StoredDomainAggregate> aggregates = new List<StoredDomainAggregate>();

            var criteria = DetachedCriteria.For<DomainAggregate>();

            try
            {
                aggregates = criteria.GetExecutableCriteria(_aggregateStorageSession.Session).List<StoredDomainAggregate>();
            }
            catch (Exception e)
            {
                throw e;
            }

            if (aggregates.Count > 0)
            {
                foreach (var aggregate in aggregates)
                {
                    var entity = _serializationProvider.Deserialize<TEntity>(aggregate.Data);
                    //entity.SetEventPublisher(_eventBus);
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public TEntity Save<TEntity>(TEntity entity) where TEntity : AbstractAggregateRoot
        {
            //entity.IsSnapshotPeriodElapsed = null;

            var rt = entity as IAggregateRoot;

            var data = _serializationProvider.SerializeToBytes(rt);

            var root = new StoredDomainAggregate()
                           {
                               Data = data,
                               Id = entity.Id,
                               Name = entity.GetType().Name,
                               Timestamp = System.DateTime.Now,
                               Version = entity.Version
                           };

            using (var txn = _aggregateStorageSession.Session.BeginTransaction())
            {
                try
                {
                    _aggregateStorageSession.Session.Save(root);
                    txn.Commit();
                }
                catch (Exception e)
                {
                    txn.Rollback();
                    throw e;
                }
            }

            //entity.SetEventPublisher(_eventBus);
            return entity;

        }


    }
}