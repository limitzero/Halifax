using System;
using System.Reflection;
using Axiom.Storage.Aggregates;
using Castle.Core.Interceptor;
using Castle.MicroKernel;
using Axiom.Eventing;

namespace Axiom.Internals
{
    public class AggregateRootApplyEventInterceptor : IInterceptor
    {
        private readonly IKernel _kernel;

        public AggregateRootApplyEventInterceptor(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            if (!invocation.Method.Name.StartsWith("ApplyEvent")) return;

            // need to push the aggregate root to storage 
            // and push the event out to the handlers for 
            // custom storage of the event details:
            PushAggregateToStorage(invocation);
            PushEventToEventHandlers(invocation);
        }

        private void PushAggregateToStorage(IInvocation invocation)
        {
            var root = invocation.InvocationTarget;
            var theType = typeof (IDomainRepository<>).MakeGenericType(root.GetType());
            var theInstance = _kernel.Resolve(theType);
            theInstance.GetType().InvokeMember("Save", BindingFlags.InvokeMethod, null, theInstance, new object[] {root});
        }

        private void PushEventToEventHandlers(IInvocation invocation)
        {
            // publish the event:
            var root = invocation.InvocationTarget as AbstractAggregateRoot;
            var domainEvent = invocation.Arguments[0] as IDomainEvent;

            using (var eventBus = _kernel.Resolve<IStartableEventBus>())
            {
                eventBus.Start();
                eventBus.Publish(domainEvent);
            }
        }
    }
}