using System;
using System.Collections.Generic;
using Halifax.Events;

namespace Halifax.Configuration.Impl.EventStorage.Impl
{
	public class NHibernateSqlServerEventStorage : IEventStorage, IDisposable
	{
		private readonly string connection;

		public NHibernateSqlServerEventStorage(string connection)
		{
			this.connection = connection;
		}

		public void Save<TEvent>(TEvent @event) where TEvent : Event
		{
			throw new NotImplementedException();
		}

		public ICollection<Event> GetHistory(Guid aggregateRootId)
		{
			throw new NotImplementedException();
		}

		public ICollection<Event> GetHistorySinceSnapshot(Guid aggregateRootId)
		{
			throw new NotImplementedException();
		}

		public ICollection<Event> GetCreationEvents()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			
		}
	}
}