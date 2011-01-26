using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Async.Pipeline.Module;
using Axiom.Async.Transport;
using Axiom.Commanding;
using Axiom.Configuration;
using Axiom.Configuration.Bootstrapper;
using Axiom.NHibernate.AggregateStorage.Tests.Domain.Products.CreateProducts;
using Castle.MicroKernel;
using Castle.Windsor;
using Xunit;
using Axiom.Storage.Aggregates;
using Axiom.NHibernate.AggregateStorage.Tests.Domain.Products;
using Castle.MicroKernel.Registration;
using Axiom.Async.Endpoints.Module;
using NHibernate;
using Axiom.Commanding.Module;
using Axiom.Storage.Events;

namespace Axiom.NHibernate.AggregateStorage.Tests
{
    public class NHibernateDomainRepositoryTests
    {
        private readonly IWindsorContainer _container;

        public NHibernateDomainRepositoryTests()
        {
            _container = new WindsorContainer(@"sample.config.xml");
            _container.AddFacility(AxiomFacility.FACILITY_ID, new AxiomFacility());
            SchemaManager.CreateSchema();
        }

        ~NHibernateDomainRepositoryTests()
        {
            _container.Dispose();
        }

        [Fact]
        public void can_save_domain_event_to_repository_and_have_event_recorded_as_such()
        {
            using(var bus = _container.Resolve<IStartableCommandBus>())
            {
                bus.Start();
                bus.Send(new CreateProductCommand("Windex","All purpose cleaner"));
            }

            var storage = _container.Resolve<IEventStorage>();
            var creationEvent = storage.GetCreationEvents().First();

            var theEvent = (from ev in storage.GetHistory(creationEvent.AggregateId)
                            where ev.GetType() == typeof (ProductCreatedEvent)
                            select ev).FirstOrDefault();

            Assert.NotNull(theEvent);
            Assert.Equal(typeof(ProductCreatedEvent), theEvent.GetType());
        }

    }

    public class NHibernateDomainRepositoryStorageBootstrapper 
        : AbstractBootstrapper
    {
        public NHibernateDomainRepositoryStorageBootstrapper()
        {
            IsActive = false;
        }
        public override void Configure()
        {
            Kernel.Register(Component.For<IDomainRepository>()
                                .ImplementedBy<NHibernateDomainRepository>());

            Kernel.Register(Component.For<NHibernateDomainRepositoryCommandBusModule>()
                                .ImplementedBy<NHibernateDomainRepositoryCommandBusModule>()
                                .LifeStyle.Singleton);
        }
    }

    public class NHibernateDomainRepositoryCommandBusModule : 
        AbstractCommandBusModule
    {
        private readonly IKernel _kernel;
        private global::NHibernate.Cfg.Configuration _configuration;
        private global::NHibernate.ISessionFactory _factory;

        public NHibernateDomainRepositoryCommandBusModule(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override void OnCommandBusStartMessagePublishing(CommandBusStartPublishMessageEventArgs args)
        {
            // register the HNibernate session for the domain repositories:
            if(_configuration == null)
            {
                _configuration = new global::NHibernate.Cfg.Configuration();
                _configuration.Configure();
            }

            if(_factory == null)
                _factory = _configuration.BuildSessionFactory();

            _kernel.AddComponentInstance(typeof(ISession).Name, typeof(ISession), _factory.OpenSession());
        }

        public override void OnCommandBusCompletedMessagePublishing(CommndBusCompletedPublishMessageEventArgs args)
        {
            try
            {
                var session = _kernel.Resolve<ISession>();
                if(session == null) return;
                session.Dispose();
            }
            catch (Exception e)
            {                
                throw e;
            }
        }

        public override void  OnCommandBusModuleDisposing()   
        {
            if(_factory != null)
            {
                _factory.Dispose();
                _factory = null;
            }
            
            if(_configuration != null)
                _configuration = null;
        }
    }
}
