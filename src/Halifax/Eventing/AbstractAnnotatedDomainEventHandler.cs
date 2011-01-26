using Axiom.Commanding;
using Axiom.Eventing;

namespace Axiom.Eventing
{
    /// <summary>
    /// Base class for use with the attribute <seealso cref="DomainEventHandlerAttribute"/>
    /// for mapping an event to a event handler.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class AbstractAnnotatedDomainEventHandler<TEvent> :
        IEventHandler<TEvent> where TEvent : class, IDomainEvent
    {
        /// <summary>
        /// Default method to record an event that has happened 
        /// to the domain entity.
        /// </summary>
        /// <param name="domainEvent">event to record for the domain entity</param>
        public abstract void Handle (TEvent domainEvent);
    }
}