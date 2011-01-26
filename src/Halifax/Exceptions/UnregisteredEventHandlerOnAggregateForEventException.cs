using System;

namespace Halifax.Exceptions
{
    public class UnregisteredEventHandlerOnAggregateForEventException : HalifaxException
    {
        private const string _message =
            "There was not an event handler defined on the aggregate root '{0}' for the event '{1}'. " +
            "Make sure to create a public method with the signature 'public On{2}({3} domainEvent)' on the aggregate root before calling the 'Apply' method " +
            " for recording the current change(s).";

        public UnregisteredEventHandlerOnAggregateForEventException(Type aggregateRoot, Type domainEvent)
            : base(string.Format(_message, aggregateRoot.FullName, domainEvent.FullName,
                                 domainEvent.Name, domainEvent.Name))
        {
        }
    }
}