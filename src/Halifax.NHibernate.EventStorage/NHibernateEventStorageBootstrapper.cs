using Castle.Core;
using Castle.Facilities.FactorySupport;
using Castle.MicroKernel.Registration;
using Halifax.Configuration.Bootstrapper;
using Halifax.Storage.Events;
using NHibernate;

namespace Halifax.NHibernate.EventStorage
{
    /// <summary>
    /// Bootstraps the configuration for using NHibernate as the 
    /// persistance store for domain events.
    /// </summary>
    public class NHibernateEventStorageBootstrapper : 
        AbstractBootstrapper
    {
        public NHibernateEventStorageBootstrapper()
        {
            this.IsActive = true;
        }

        public override void Configure()
        {
            // trigger the storage of the messages to open a connection to 
            // the datastore when the event bus is processing messages:
            Kernel.Register(Component.For<NHibernateEventStorageEventBusModule>()
                                .ImplementedBy<NHibernateEventStorageEventBusModule>());

            // configure the main NHibernate configuration (only once)
            // and trigger the connection to the session from the static 
            // property on the bus module injected with the session factory:
            var configuration = new global::NHibernate.Cfg.Configuration();
            configuration.Configure();

            var sessionFactory = configuration.BuildSessionFactory();

            Kernel.AddFacility("factory", new FactorySupportFacility());

            Kernel.Register(Component.For<ISessionFactory>().Instance(sessionFactory));

            // wrap the ISession in a local implementation:
            Kernel.Register(Component.For<IEventStorageSession>().
                                UsingFactoryMethod( () => 
                                    Kernel.Resolve<NHibernateEventStorageEventBusModule>().GetCurrentSession())
                                .LifeStyle.Is(LifestyleType.Transient));

            // configure the NHibernate event storage provider implementation 
            // (must be added after the factory support method for the container to 
            // properly resolve the on-demand component):
            Kernel.Register(Component.For<IEventStorage>()
                                .ImplementedBy<NHibernateEventStorage>());
        }
    }

}