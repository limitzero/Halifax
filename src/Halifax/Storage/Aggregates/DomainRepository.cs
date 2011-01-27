using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Halifax.Eventing;
using Halifax.Events;
using Halifax.Storage.Events;

namespace Halifax.Storage.Aggregates
{
    /// <summary>
    /// Repository charged with creating and finding aggregate roots of a particular type and 
    /// reconsituting them from their associated events.
    /// </summary>
    public class DomainRepository : IDomainRepository
    {
        private readonly IEventStorage _eventStorage;
        private readonly IKernel _kernel;

        public DomainRepository(
            IKernel kernel,
            IEventStorage eventStorage)
        {
            _kernel = kernel;
            _eventStorage = eventStorage;
        }

        public TEntity Create<TEntity>() where TEntity : AbstractAggregateRoot, new()
        {
            var entity = _kernel.Resolve<TEntity>();
            entity.Id = Guid.NewGuid();

            var ev = new AggregateCreatedEvent
                         {
                             AggregateId = entity.Id,
                             Aggregate = entity.GetType().FullName,
                             EventDateTime = DateTime.Now
                         };

            _eventStorage.Save(ev);

            return entity;
        }

        public TEntity Find<TEntity>(Guid Id) where TEntity : AbstractAggregateRoot
        {
            var entity = _kernel.Resolve<TEntity>();

            // need to reconsitute the entity from the last snapshot 
            // or generate from full history if the snapshot is not present:
            ICollection<IDomainEvent> history = _eventStorage.GetHistorySinceSnapshot(Id);
            if (history.Count == 0)
                history = _eventStorage.GetHistory(Id);

            if (history.Count > 0)
                entity.LoadFromHistory(history);

            return entity;
        }

    }
}