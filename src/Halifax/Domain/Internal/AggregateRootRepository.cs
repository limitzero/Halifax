using System;
using System.Collections.Generic;
using Halifax.Configuration;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Events;
using Halifax.Internals.Events;
using Halifax.Read;

namespace Halifax.Domain.Internal
{
	/// <summary>
	/// Repository charged with creating and finding aggregate roots of a particular type and 
	/// reconsituting them from their associated events. The repository supports both CQRS+ES and 
	/// CQRS + DM.
	/// </summary>
	public class AggregateRootRepository : IAggregateRootRepository
	{
		private readonly IContainer container;

		public AggregateRootRepository(IContainer container)
		{
			this.container = container;
		}

		public TAggregateRoot Get<TAggregateRoot>() where TAggregateRoot : AggregateRoot
		{
			return this.Get<TAggregateRoot>(Guid.Empty);
		}

		public TAggregateRoot Get<TAggregateRoot>(Guid id) where TAggregateRoot : AggregateRoot
		{
			var aggregate_root = this.container.Resolve<TAggregateRoot>();

			// we are creating a new instance if not already noted by an existing id:
			if (id == Guid.Empty)
				id = CombGuid.NewGuid(); 

			if (typeof(IReadModel).IsAssignableFrom(aggregate_root.GetType()) == true)
			{
				// reconsituting aggregate from persistant storage, not event storage (DM):
				aggregate_root = this.FetchFromRepository<TAggregateRoot>(id);
			}
			else
			{
				// reconsitute aggregate from event storage, not persistant storage (ES):
				aggregate_root = this.FetchFromEventStorage<TAggregateRoot>(id);
			}

			return aggregate_root;
		}

		private TAggregateRoot FetchFromRepository<TAggregateRoot>(Guid id)
			where TAggregateRoot : AggregateRoot
		{
			var aggregate_root = this.container.Resolve<TAggregateRoot>();

			var repository_type = typeof(IReadModelRepository<>).MakeGenericType(aggregate_root.GetType());
			var repository = this.container.Resolve(repository_type);

			if (repository == null) return aggregate_root;

			var read_model = aggregate_root as IReadModel;

			if (read_model == null) return aggregate_root;

			var model = repository.GetType().GetMethod("Get")
					.Invoke(repository, new object[] {id});

			if(model != null && typeof(AggregateRoot).IsAssignableFrom(model.GetType()))
			{
				aggregate_root = model as TAggregateRoot;
			}
			else
			{
				// creating aggregate for the first time:
				aggregate_root.Id = id;
				NoteAggregateCreatedForFirstTime<TAggregateRoot>(id);
			}

			return aggregate_root;
		}

		private TAggregateRoot FetchFromEventStorage<TAggregateRoot>(Guid id)
			where TAggregateRoot : AggregateRoot
		{
			var aggregate_root = this.container.Resolve<TAggregateRoot>();

			// need to reconsitute the entity from the last snapshot 
			// or generate from full history if the snapshot is not present:
			IEventStorage event_storage = this.container.Resolve<IEventStorage>();

			ICollection<Event> history = event_storage.GetHistorySinceSnapshot(id);

			if (history.Count == 0)
				history = event_storage.GetHistory(id);

			if (history.Count > 0)
				aggregate_root.LoadFromHistory(history);
			else
			{
				// creating aggregate for the first time:
				NoteAggregateCreatedForFirstTime<TAggregateRoot>(id);
			}

			aggregate_root.Id = id;

			return aggregate_root;
		}

		private void NoteAggregateCreatedForFirstTime<TAggregateRoot>(Guid id)
			where TAggregateRoot : AggregateRoot
		{
			IEventStorage event_storage = this.container.Resolve<IEventStorage>();

			// creating aggregate for the first time:
			var ev = new AggregateCreatedEvent
			{
				EventSourceId = id,
				Aggregate = typeof(TAggregateRoot).FullName,
				At = DateTime.Now,
				From = typeof(TAggregateRoot).FullName
			};

			event_storage.Save(ev);
		}
	}
}