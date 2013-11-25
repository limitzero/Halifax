using System;
using Halifax.Events;
using Halifax.Internals.Exceptions;
using Halifax.Internals.Reflection;

namespace Halifax.Internals.Dispatchers.Impl
{
    public class EventMessageDispatcher : IEventMessageDispatcher
    {
        private readonly IReflection _reflection;

        public EventMessageDispatcher(IReflection reflection)
        {
            _reflection = reflection;
        }

        #region IEventMessageDispatcher Members

        public void Dispatch(Event @event)
        {
            try
            {
                _reflection.InvokeHandleMethodForEventConsumer(@event);
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