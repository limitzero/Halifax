using System;
using Halifax.Configuration.Impl.EventStorage.Impl;
using Halifax.Configuration.Impl.EventStorage.Impl.NHibernate;
using Halifax.Configuration.Impl.Extensibility.NHibernate;
using Halifax.Read;

namespace Halifax.Configuration.Impl.EventStorage
{
	/// <summary>
	/// Marker for the storage options for event storage.
	/// </summary>
	public class EventStorageOptions
	{
		private readonly IContainer container;

		public EventStorageOptions(IContainer container)
		{
			this.container = container;
		}

		/// In this configuration, a no-op event storage is used in the implementation of an aggregate root 
		/// in which the events are not saved to storage for the express purpose 
		/// of hydrating the state of the aggregate root since the last snapshot. In essence 
		/// the aggregate root is a read model that can be persisted and read back with exposed
		/// state for manipulation. Make sure that all aggregate roots implement the <seealso cref="IReadModel"/>
		/// interface in order to be stored and retrieved back from persistant storage.
		public EventStorageOptions UsingNoStorage()
		{
			this.container.Register<IEventStorage, NullEventStorage>();
			return this;
		}

		/// <summary>
		/// Use the in-memory (volatile) storage for events emitted from the aggregate root behavioral model
		/// </summary>
		/// <returns></returns>
		public EventStorageOptions UsingInMemoryStorage()
		{
			this.container.Register<IEventStorage, InMemoryEventStorage>();
			return this;
		}

		/// <summary>
		/// Use NHibernate as a proxy to store events emitted from the aggregate root behavioral model
		/// (all events will be deserialized into one local table for retrieval and storage)
		/// </summary>
		/// <returns></returns>
		public EventStorageOptions UsingNHibernate(Func<NHibernateConfiguration, NHibernateConfiguration> storage_configuration)
		{
			var event_storage_configuration = storage_configuration(new NHibernateConfiguration());
			var event_storage = new NHibernateEventStorageConfiguration(event_storage_configuration);
			this.container.RegisterInstance<NHibernateEventStorageConfiguration>(event_storage);
			return this;
		}
	}
}