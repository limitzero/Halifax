using System;

namespace Halifax.Internals.Exceptions
{
    public class MissingExternalEventHandlerForEventException : HalifaxException
    {
        private const string _message =
            "There was not an external event handler defined to handle the message '{0}'.";

        public MissingExternalEventHandlerForEventException(Type domainEvent)
            : base(string.Format(_message, domainEvent.FullName))
        {
        }
    }
}