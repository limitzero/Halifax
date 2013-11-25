using System;
using System.Collections.Generic;
using System.Transactions;
using Halifax.Configuration.Impl.Eventing;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Domain;
using Halifax.Events;

namespace Halifax.Internals
{
	/// <summary>
	/// Class to take all of the events created by an aggregate
	/// and persist them to a storage medium (optional) and lastly 
	/// call the event handlers to do something meaningful with 
	/// the event.
	/// </summary>
    public class TransactedSession : ITransactedSession
    {
        private readonly IEventStorage event_storage;
        private readonly IEventBus event_bus;
        private readonly AggregateRoot aggregate_root;
        private readonly TransactionScopeOption transaction_scope_option;

        public TransactedSession(
            IEventStorage eventStorage,
            IEventBus eventBus,
            AggregateRoot aggregateRoot)
            : this(eventStorage, 
			eventBus, 
			aggregateRoot, 
			TransactionScopeOption.RequiresNew)
        {
        }

        public TransactedSession(
            IEventStorage eventStorage,
            IEventBus eventBus,
            AggregateRoot aggregateRoot,
            TransactionScopeOption transactionScopeOption)
        {
            event_storage = eventStorage;
            event_bus = eventBus;
            aggregate_root = aggregateRoot;
            transaction_scope_option = transactionScopeOption;
        }

        public void Dispose()
        {
            this.Commit();
        }

        public void Commit()
        {
        	var options = new TransactionOptions();
        	options.IsolationLevel = IsolationLevel.ReadCommitted;

			using (var txn = new TransactionScope(transaction_scope_option, options))
            {
                    var changes = new List<Event>(aggregate_root.GetChanges());

                    // commit the changes to the event store:
                    Array.ForEach(changes.ToArray(),
								  (@event) => event_storage.Save(@event));

                    // publish the events (state changes) to the custom handlers:
                    Array.ForEach(changes.ToArray(),
								  (@event) => event_bus.Publish(@event));

                    txn.Complete();  
            }
        }
    }
}