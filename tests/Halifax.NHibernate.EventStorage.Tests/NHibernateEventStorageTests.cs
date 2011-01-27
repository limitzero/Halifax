using System;
using System.Linq;
using Castle.Windsor;
using Halifax.Bus.Commanding;
using Halifax.Bus.Eventing;
using Halifax.Configuration;
using Halifax.NHibernate.EventStorage.Tests.Domain.Products.CreateProducts;
using Halifax.NHibernate.EventStorage.Tests.Events;
using Halifax.Storage.Events;
using Xunit;

namespace Halifax.NHibernate.EventStorage.Tests
{
    public class NHibernateEventStorageTests : IDisposable
    {
        private IWindsorContainer _container;

        public NHibernateEventStorageTests()
        {
            _container = new WindsorContainer(@"halifax.config.xml");
            _container.AddFacility(HalifaxFacility.FACILITY_ID, new HalifaxFacility());
            SchemaManager.CreateSchema();
        }

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }

        [Fact]
        public void can_save_domain_event_to_repository_and_retrieve_it_from_storage()
        {
            using(var bus = _container.Resolve<IStartableEventBus>())
            {
                var id = Guid.NewGuid();

                var ev = new SampleEvent()
                {
                    AggregateId = id,
                    EventDateTime = System.DateTime.Now,
                    Version = 1,
                    Who = "me"
                };

                bus.Publish(ev);

                var storage = _container.Resolve<IEventStorage>();
                Assert.Equal(typeof (SampleEvent), storage.GetHistory(id).First().GetType());
            }
        }

        [Fact]
        public void can_create_product_aggreate_root_and_record_the_product_creation_event_from_the_issued_command()
        {
            using (var bus = _container.Resolve<IStartableCommandBus>())
            {
                bus.Start();
                bus.Send(new CreateProductCommand("Windex", "All purpose cleaner"));
            }

            var storage = _container.Resolve<IEventStorage>();
            var creationEvent = storage.GetCreationEvents().First();

            var theEvent = (from ev in storage.GetHistory(creationEvent.AggregateId)
                            where ev.GetType() == typeof(ProductCreatedEvent)
                            select ev).FirstOrDefault();

            Assert.NotNull(theEvent);
            Assert.Equal(typeof(ProductCreatedEvent), theEvent.GetType());
        }

    }
}
