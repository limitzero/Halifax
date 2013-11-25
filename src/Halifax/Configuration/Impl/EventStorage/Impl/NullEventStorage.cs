using System;
using System.Collections.Generic;
using Halifax.Events;

namespace Halifax.Configuration.Impl.EventStorage.Impl
{
	/// <summary>
	/// The null event storage is used in the implementation of an aggregate root 
	/// in which the events are not saved to storage for the express purpose 
	/// of hydrating the state of the aggregate root since the last snapshot. In essence 
	/// the aggregate root is a read model that can be persisted and read back with exposed
	/// state for manipulation.
	/// </summary>
	public class NullEventStorage : IEventStorage
	{
		public void Save<TEvent>(TEvent domainEvent) where TEvent : Event
		{
		}

		public ICollection<Event> GetHistory(Guid aggregateRootId)
		{
			return new List<Event>();
		}

		public ICollection<Event> GetHistorySinceSnapshot(Guid aggregateRootId)
		{
			return new List<Event>();
		}

		public ICollection<Event> GetCreationEvents()
		{
			return new List<Event>();
		}
	}
}