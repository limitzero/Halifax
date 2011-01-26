using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Commanding;
using Axiom.Commanding.Module;
using Axiom.Configuration;
using Castle.Windsor;
using Xunit;
using Axiom.NHibernate.EventStorage.Tests.Events;
using Axiom.Internals.Serialization;
using Axiom.Storage.Events;

namespace Axiom.NHibernate.EventStorage.Tests
{
    public class NHibernateEventStorageTests
    {
        private readonly IWindsorContainer _container;

        public NHibernateEventStorageTests()
        {
            _container = new WindsorContainer(@"sample.config.xml");
            _container.AddFacility(AxiomFacility.FACILITY_ID, new AxiomFacility());
            SchemaManager.CreateSchema();
        }

        ~NHibernateEventStorageTests()
        {
            _container.Dispose();
        }

        [Fact]
        public void can_save_domain_event_to_repository()
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

    }
}
