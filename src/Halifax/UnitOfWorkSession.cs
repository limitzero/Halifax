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
    /// custom event consumers via the event bus.
    /// </summary>
    public class UnitOfWorkSession : IUnitOfWorkSession
    {
        private readonly IStartableEventBus _eventBus;
        private readonly IEventStorage _eventStorage;

        public UnitOfWorkSession(IEventStorage eventStorage,
                                 IStartableEventBus eventBus)
        {
            _eventStorage = eventStorage;
            _eventBus = eventBus;
        }

        #region IUnitOfWorkSession Members

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

        #endregion
    }
}