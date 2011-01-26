using System;
using Halifax.Eventing;
using Halifax.Storage.Events;

namespace Halifax.Storage.Internals
{
    /// <summary>
    /// Default in-memory storage of all events to the in-memory storage location.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete]
    public class InMemoryMatchAllEventHandler<T>
        : EventConsumer.For<T> where T : DomainEvent
    {
        private readonly IEventStorage _storage;

        public InMemoryMatchAllEventHandler(IEventStorage storage)
        {
            _storage = storage;
        }

        #region For<T> Members

        public void Handle(T @event)
        {
            _storage.Save(@event);
        }

        #endregion
    }
}