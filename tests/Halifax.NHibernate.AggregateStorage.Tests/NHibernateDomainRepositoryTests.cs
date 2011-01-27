using System;
using System.Linq;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Halifax.Bus.Commanding;
using Halifax.Commanding;
using Halifax.Commanding.Module;
using Halifax.Configuration;
using Halifax.Configuration.Bootstrapper;
using Halifax.NHibernate.AggregateStorage.Tests.Domain.Products;
using Halifax.NHibernate.AggregateStorage.Tests.Domain.Products.CreateProducts;
using Halifax.Storage.Aggregates;
using Halifax.Storage.Events;
using NHibernate;
using Xunit;

namespace Halifax.NHibernate.AggregateStorage.Tests
{
    public class NHibernateDomainRepositoryTests : IDisposable
    {
        private Halifax.Configuration.Infrastructure.HalifaxContext _context;
        private IWindsorContainer _container;

        public static AbstractAggregateRoot _theAggregate; 

        public NHibernateDomainRepositoryTests()
        {
            _container = new WindsorContainer(@"halifax.config.xml");
            _container.AddFacility(Halifax.Configuration.HalifaxFacility.FACILITY_ID, new HalifaxFacility());

            SchemaManager.CreateSchema();
        }

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }

        [Fact(Skip="The infrastructure should not stor aggregates, only the events of the aggregates...")]
        public void can_create_aggreate_root_and_record_the_creation_event()
        {
            IDomainRepository repository = _container.Resolve<IDomainRepository>();

            Assert.IsType<NHibernateDomainRepository>(repository);

            var product = repository.Create<Product>();
            var id = product.Id;

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


        public class MyCommand : Command
        {}

        public class MyCommandConsumer : 
            CommandConsumer.For<MyCommand>
        {
            private readonly IDomainRepository _repository;

            public MyCommandConsumer(IDomainRepository repository)
            {
                _repository = repository;
            }

            public override void Execute(IUnitOfWork session, MyCommand theCommand)
            {
                var aggregate = _repository.Create<MyAggregate>();
                _theAggregate = aggregate;
            }
        }

        public class MyAggregate : AbstractAggregateRootByConvention
        {
            public void DoSomething()
            {
            }
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
