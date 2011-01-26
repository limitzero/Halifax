using Castle.Core;
using Castle.Facilities.FactorySupport;
using Castle.MicroKernel.Registration;
using Halifax.Configuration.Bootstrapper;
using Halifax.Storage.Aggregates;
using NHibernate;

namespace Halifax.NHibernate.AggregateStorage
{
    /// <summary>
    /// Bootstraps the configuration for using NHibernate as the 
    /// persistance store for domain events.
    /// </summary>
    public class NHibernateAggregateStorageBootstrapper : 
        AbstractBootstrapper
    {
        public NHibernateAggregateStorageBootstrapper()
        {
            this.IsActive = true;
        }

        public override void Configure()
        {
            // trigger the storage of the messages to open a connection to 
            // the datastore when the event bus is processing messages:
            Kernel.Register(Component.For<NHibernateAggregateStorageSessionBusModule>()
                                .ImplementedBy<NHibernateAggregateStorageSessionBusModule>());

            // configure the main NHibernate configuration (only once)
            // and trigger the connection to the session from the static 
            // property on the bus module injected with the session factory:
            var configuration = new global::NHibernate.Cfg.Configuration();
            configuration.Configure();

            var sessionFactory = configuration.BuildSessionFactory();

            Kernel.AddFacility("factory", new FactorySupportFacility());

            Kernel.Register(Component.For<ISessionFactory>().Instance(sessionFactory));

            // wrap the ISession in a local implementation:
            Kernel.Register(Component.For<IAggregateStorageSession>().
                                UsingFactoryMethod( () =>
                                    Kernel.Resolve<NHibernateAggregateStorageSessionBusModule>().GetCurrentSession())
                                .LifeStyle.Is(LifestyleType.Transient));

            // configure the NHibernate aggregate storage provider implementation 
            // (must be added after the factory support method for the container to 
            // properly resolve the on-demand component):
            Kernel.Register(Component.For<IDomainRepository>()
                                .ImplementedBy<NHibernateDomainRepository>());
        }
    }

}