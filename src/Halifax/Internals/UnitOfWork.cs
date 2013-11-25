using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Halifax.Configuration;
using Halifax.Configuration.Impl.Eventing;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Domain;
using Halifax.Events;
using Halifax.Read;

namespace Halifax.Internals
{
	/// <summary>
	/// The unit of work marks a consistency boundary in the processing 
	/// of the command to the command consumer and the streaming 
	/// of events to the indicated sources for handling.
	/// </summary>
	public class UnitOfWork : IUnitOfWork
	{
		private readonly IContainer container;

		public UnitOfWork(IContainer container)
		{
			this.container = container;
		}

		public void Accept(AggregateRoot root)
		{
			if(root == null) return;

			var options = new TransactionOptions();
			options.IsolationLevel = IsolationLevel.ReadCommitted;

			using (var txn = new TransactionScope(TransactionScopeOption.RequiresNew, options))
			{
				InvokeDomainModelPersistanceForAggregate(root);
				InvokeEventSourcingForAggregate(root);
				txn.Complete();
			}
		}

		// save the aggregate in the persistance store, if it is used as a read model (CQRS + DM/DE):
		private void InvokeDomainModelPersistanceForAggregate(AggregateRoot aggregateRoot)
		{
			if (typeof(IReadModel).IsAssignableFrom(aggregateRoot.GetType()) == false)
				return;

			var repositoryType = typeof(IReadModelRepository<>).MakeGenericType(aggregateRoot.GetType());
			var repository = this.container.Resolve(repositoryType);

			if (repository == null) return;

			var readModel = aggregateRoot as IReadModel;

			if (readModel == null) return;

			if (((IReadModel)aggregateRoot).Id != Guid.Empty)
			{
				Delegate updateDelegate = Delegate.CreateDelegate(typeof(Action<>)
					.MakeGenericType(new Type[] { readModel.GetType() }),
					repository, "Update");
				updateDelegate.DynamicInvoke(readModel);
			}
			else 
			{
				((IReadModel)aggregateRoot).Id = CombGuid.NewGuid();
				Delegate insertDelegate = Delegate.CreateDelegate(typeof (Action<>)
				        .MakeGenericType(new Type[] {readModel.GetType()}),
				        repository, "Insert");
				insertDelegate.DynamicInvoke(readModel);
			}
		}

		// save the events in the persistance store and fire event handlers (if configured) (CQRS + ES):
		private void InvokeEventSourcingForAggregate(AggregateRoot aggregateRoot)
		{
			var events = new List<Event>(aggregateRoot.GetChanges());
			this.StreamEventsToStorage(events);
			this.StreamEventsToHandlers(events);
		}

		private void StreamEventsToStorage(IEnumerable<Event> events)
		{
			var eventStorage = this.container.Resolve<IEventStorage>();
			Array.ForEach(events.ToArray(), eventStorage.Save);
		}

		private void StreamEventsToHandlers(IEnumerable<Event> events)
		{
			var eventBus = this.container.Resolve<IEventBus>();
			Array.ForEach(events.ToArray(), @event => eventBus.Publish(@event));
		}
	}
}