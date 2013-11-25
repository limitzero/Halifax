using Halifax.Configuration;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Configuration.Impl.EventStorage.Impl.NHibernate;
using Halifax.Configuration.Impl.Repository.Impl;
using Halifax.NHibernate.EventStore;
using Halifax.NHibernate.EventStore.Impl;
using Halifax.NHibernate.ReadModel;
using Halifax.NHibernate.ReadModel.Impl;
using Halifax.Read;

namespace Halifax.NHibernate
{
    /// <summary>
    /// Configures the underlying container to use NHibernate for the event 
    /// store and persistance model for reads.
    /// </summary>
    public class NHibernateConfigurator : ICanConfigureContainer
    {
        public void Configure(IContainer container)
        {
			// create the session factory object for the event store and open the session on every
			// data access point to the persistance store:
			BuildSessionFactoryEventStore(container);

			// create the session factory object for the read model and open the session on every
			// data access point to the persistance store:
			BuildSessionFactoryForReadModel(container);
        }

		private static void BuildSessionFactoryEventStore(IContainer container)
		{
			var cfg = new global::NHibernate.Cfg.Configuration();

			var nhibernate_event_store_configuration = container.Resolve<NHibernateEventStorageConfiguration>();

			if (nhibernate_event_store_configuration != null)
			{
				foreach (var property in nhibernate_event_store_configuration.NHibernateConfiguration.GetProperties())
				{
					cfg.SetProperty(property.Key, property.Value.ToString());
				}

				cfg.AddAssembly(nhibernate_event_store_configuration.NHibernateConfiguration.MappingAssembly);

				var event_store_session_factory = new NHibernateEventStoreSessionFactory(cfg)
				{
					Factory = cfg.BuildSessionFactory()
				};

				container.RegisterInstance<INHibernateEventStoreSessionFactory>(event_store_session_factory);

				container.Register<IEventStorage, NHibernateEventStorage>();

				container.Register<INHibernateEventStoreSchemaManager, NHibernateEventStoreSchemaManager>();
			}
		}

    	private static void BuildSessionFactoryForReadModel(IContainer container)
		{
			var cfg = new global::NHibernate.Cfg.Configuration();

			var nhibernate_read_model_configuration = container.Resolve<NHibernateReadModelConfiguration>();

			if (nhibernate_read_model_configuration != null)
			{
				foreach (var property in nhibernate_read_model_configuration.NHibernateConfiguration.GetProperties())
				{
					cfg.SetProperty(property.Key, property.Value.ToString());
				}

				cfg.AddAssembly(nhibernate_read_model_configuration.NHibernateConfiguration.MappingAssembly);

				var read_model_session_factory = new NHibernateReadModelSessionFactory(cfg)
				                                 	{
				                                 		Factory = cfg.BuildSessionFactory()
				                                 	};

				container.RegisterInstance<INHibernateReadModelSessionFactory>(read_model_session_factory);

				container.Register<INHibernateReadModelSchemaManager, NHibernateReadModelSchemaManager>();
				container.Register(typeof (IReadModelRepository<>), typeof (NHibernateReadModelRepository<>));
			}
		}
    }

}