using System;

namespace Axiom.Eventing
{
    /// <summary>
    /// Attribute attached to a event handler class 
    /// for allowing the event handler to use any 
    /// void method return type method signature to 
    /// process an event that has happened to a domain entity.
    /// </summary>
    /// <example>
    /// [EventHandler]
    /// public class OrderBilledEventHandler : AnnotatedEventHandler{OrderBilledEvent}
    /// {
    ///     public override void Handle(OrderBilledEvent domainEvent)
    ///     {
    ///     }
    /// }
    /// </example>
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DomainEventHandlerAttribute : Attribute
    {
        
    }
}