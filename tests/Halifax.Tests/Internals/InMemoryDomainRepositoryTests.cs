using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Halifax.Domain;
using Xunit;

namespace Halifax.Tests.Internals
{
    public class InMemoryDomainRepositoryTests
    {
        private readonly IWindsorContainer _container;
        private readonly IAggregateRootRepository root_repository;
        
        public InMemoryDomainRepositoryTests()
        {
            _container = IoC.BuildContainer();
            _container.Kernel.Register(Component.For<TestEntity>().ImplementedBy<TestEntity>());
            root_repository = _container.Resolve<IAggregateRootRepository>();
        } 
      
        [Fact]
        public void can_create_entity_and_the_entity_identifer_be_set_to_a_new_guid()
        {
            var entity = root_repository.Create<TestEntity>();
            Assert.True(entity.Id != Guid.Empty);
        }

        [Fact]
        public void can_retrieve_entity_by_identifier_that_is_reconstituted_from_event_history()
        {
            var entity = root_repository.Create<TestEntity>();

            var fromStorage = root_repository.Find<TestEntity>(entity.Id);

            Assert.Equal(entity.Id, fromStorage.Id);
        }
    }
}