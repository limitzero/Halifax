using Axiom.Storage.Aggregates;
using Castle.Windsor;
using System.Reflection;
namespace Axiom.Eventing
{
    public class GlobalEventProvider
    {
        private static IWindsorContainer _container;

        static GlobalEventProvider()
        {
        }

        public static void Initialize(IWindsorContainer container)
        {
            if (_container == null)
                _container = container;
        }

        public static void Publish<TAggregate, TEvent>(TAggregate root, TEvent domainEvent)
            where TAggregate : AbstractAggregateRoot, new() 
            where TEvent : class, IDomainEvent
        {
            // persist the root:
            var theType = typeof (IDomainRepository<>).MakeGenericType(typeof (TAggregate));
            var theInstance = _container.Resolve(theType);
            theInstance.GetType().InvokeMember("Save", BindingFlags.InvokeMethod, 
                null, theInstance, new object[] {root});

            // publish the event:
            using(var eventBus = _container.Resolve<IStartableEventBus>())
            {
                eventBus.Start();
                eventBus.Publish(root, domainEvent);
            }
        }
    }
}