using System;
using System.Collections.Generic;
using System.Transactions;
using Halifax.Bus.Eventing;
using Halifax.Eventing;
using Halifax.Storage.Events;

namespace Halifax
{
    /// <summary>
    /// Default unit of work session to persist the events
    /// to the event store and send the events to the 
    /// custom event consumers via the event bus. This is 
    /// injected into every command consumer to ensure
    /// consistency in the proliferation of events to the storage
    /// mechanism.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IStartableEventBus _eventBus;
        private readonly IEventStorage _eventStorage;

        public UnitOfWork(IEventStorage eventStorage,
                                 IStartableEventBus eventBus)
        {
            _eventStorage = eventStorage;
            _eventBus = eventBus;
        }

        public object CurrentCommand { get; set; }

        public void Accept(AbstractAggregateRoot root)
        {
            using (var txn = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    var changes = new List<IDomainEvent>(root.GetChanges());

                    // commit the changes to the event store:
                    Array.ForEach(changes.ToArray(),
                                  (theEvent) => _eventStorage.Save(theEvent as DomainEvent));

                    // publish the events (changes) to the custom handlers:
                    Array.ForEach(changes.ToArray(),
                                  (theEvent) => _eventBus.Publish(theEvent as DomainEvent));

                    txn.Complete();
                }
                catch (Exception e)
                {
                }
            }
        }

        public ITransactedSession BeginTransaction(AbstractAggregateRoot root)
        {
            return BeginTransaction(root, TransactionScopeOption.RequiresNew);
        }

        public ITransactedSession BeginTransaction(AbstractAggregateRoot root, TransactionScopeOption option)
        {
            return new TransactedSession(_eventStorage, _eventBus, root, option);
        }
    }
}