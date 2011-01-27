using System;
using System.Collections.Generic;
using System.Transactions;
using Halifax.Bus.Eventing;
using Halifax.Eventing;
using Halifax.Storage.Events;

namespace Halifax
{
    public class TransactedSession : ITransactedSession
    {
        private readonly IEventStorage _eventStorage;
        private readonly IStartableEventBus _eventBus;
        private readonly AbstractAggregateRoot _aggregateRoot;
        private readonly TransactionScopeOption _option;

        public TransactedSession(
            IEventStorage eventStorage,
            IStartableEventBus eventBus,
            AbstractAggregateRoot aggregateRoot)
            : this(eventStorage, eventBus, aggregateRoot, TransactionScopeOption.RequiresNew)
        {
        }

        public TransactedSession(
            IEventStorage eventStorage,
            IStartableEventBus eventBus,
            AbstractAggregateRoot aggregateRoot,
            TransactionScopeOption option)
        {
            _eventStorage = eventStorage;
            _eventBus = eventBus;
            _aggregateRoot = aggregateRoot;
            _option = option;
        }

        public void Dispose()
        {
            this.Commit();
        }

        public void Commit()
        {
            using (var txn = new TransactionScope(_option))
            {
                try
                {
                    var changes = new List<IDomainEvent>(_aggregateRoot.GetChanges());

                    // commit the changes to the event store:
                    Array.ForEach(changes.ToArray(),
                                  (theEvent) => _eventStorage.Save(theEvent as DomainEvent));

                    // publish the events (state changes) to the custom handlers:
                    Array.ForEach(changes.ToArray(),
                                  (theEvent) => _eventBus.Publish(theEvent as DomainEvent));

                    txn.Complete();
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}