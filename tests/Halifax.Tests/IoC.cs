using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Halifax;
using Halifax.Bus.Commanding;
using Halifax.Commanding;
using Halifax.Eventing;
using Halifax.Internals.Dispatchers;
using Halifax.Internals.Reflection;
using Halifax.Storage.Aggregates;
using Halifax.Storage.Events;
using Halifax.Bus.Eventing;

namespace Halifax.Tests
{
    public class IoC
    {
        static IoC()
        {
        }

        public static IWindsorContainer BuildContainer()
        {
            var container = new WindsorContainer();

            container.Register(Component.For<IStartableCommandBus>()
                                   .ImplementedBy<InProcessCommandBus>());

            container.Register(Component.For<IStartableEventBus>()
                                   .ImplementedBy<InProcessEventBus>());

            container.Register(Component.For<ICommandMessageDispatcher>()
                                   .ImplementedBy<CommandMessageDispatcher>());

            container.Register(Component.For<IReflection>()
                                   .ImplementedBy<DefaultReflection>());

            container.Register(Component.For<IEventMessageDispatcher>()
                                   .ImplementedBy<EventMessageDispatcher>());

            container.Register(Component.For<IEventStorage>()
                                   .ImplementedBy<InMemoryEventStorage>());

            // set the storage for the domain aggregates (in-memory):
            container.Kernel.AddComponent(typeof(IDomainRepository).Name,
                                          typeof(IDomainRepository),
                                          typeof(DomainRepository));


            return container;
        }

        
    }
}