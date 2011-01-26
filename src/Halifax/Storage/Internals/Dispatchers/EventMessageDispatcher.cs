using System;
using Halifax.Eventing;
using Halifax.Exceptions;
using Halifax.Storage.Internals.Reflection;

namespace Halifax.Storage.Internals.Dispatchers
{
    public class EventMessageDispatcher : IEventMessageDispatcher
    {
        private readonly IReflection _reflection;

        public EventMessageDispatcher(IReflection reflection)
        {
            _reflection = reflection;
        }

        #region IEventMessageDispatcher Members

        public void Dispatch<TEVENT>(TEVENT domainEvent)
            where TEVENT : class, IDomainEvent
        {
            try
            {
                _reflection.InvokeHandleMethodForEventConsumer(domainEvent);
            }
            catch (Exception e)
            {
                // this is OK..some events will not need to be handled externally:
                if (e is MissingExternalEventHandlerForEventException) return;

                Exception toThrow = e;

                while (e != null)
                {
                    toThrow = e;
                    e = e.InnerException;
                }

                throw toThrow;
            }
        }

        #endregion
    }
}