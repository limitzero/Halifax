using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using Halifax.Bus.Eventing;
using Halifax.Eventing;
using Halifax.Events;
using Halifax.Exceptions;

namespace Halifax
{
    [Obsolete]
    public abstract class BaseEntity :
        IEntity,
        IEventProvider,
        ISnapshotable,
        IEventPublisher
    {
        [XmlIgnore] private readonly List<IDomainEvent> _recordedEvents;
        [XmlIgnore] private readonly Dictionary<Type, Action<IDomainEvent>> _registeredEvents;
        [XmlIgnore] private IEventBus _publisher;

        protected BaseEntity()
        {
            if (_recordedEvents == null)
                _recordedEvents = new List<IDomainEvent>();

            if (_registeredEvents == null)
                _registeredEvents = new Dictionary<Type, Action<IDomainEvent>>();
        }

        [XmlIgnore]
        public Func<bool> IsSnapshotPeriodElapsed { get; set; }

        public int Version { get; set; }

        #region IEntity Members

        public Guid Id { get; set; }

        #endregion

        #region IEventProvider Members

        public virtual void Clear()
        {
            _recordedEvents.Clear();
        }

        public virtual void Record(IDomainEvent domainEvent)
        {
            _recordedEvents.Add(domainEvent);

            ICollection<IDomainEvent> changes = GetChanges();

            if (IsSnapshotPeriodElapsed != null)
            {
                if (IsSnapshotPeriodElapsed())
                    changes = GetChanges();
            }
            else
            {
                foreach (IDomainEvent change in GetChanges())
                    _publisher.Publish(change);
            }
        }

        public virtual ICollection<IDomainEvent> GetChanges()
        {
            return _recordedEvents.AsReadOnly();
        }

        public virtual void LoadFromHistory(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                if (typeof (LifecycleDomainEvent).IsAssignableFrom(domainEvent.GetType())) continue;

                domainEvent.AggregateId = Id;
                domainEvent.Version = Version;
                Apply(domainEvent, true);
            }
        }

        #endregion

        #region IEventPublisher Members

        public virtual void SetEventPublisher(IEventBus publisher)
        {
            _publisher = publisher;
        }

        #endregion

        #region ISnapshotable Members

        public virtual int GetRecordedEventsCount()
        {
            return _recordedEvents.Count;
        }

        public virtual void TakeSnapshot()
        {
        }

        #endregion

        /// <summary>
        /// This will register all of the events that will cause a change to the aggregate root.
        /// </summary>
        public virtual void RegisterEvents()
        {
        }

        /// <summary>
        /// This will apply the given domain event to the domain entity
        /// and subsequently look for the public event method on the 
        /// domain entity to carry out the resulting domain logic 
        /// for the event.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to apply to the domain entity or domain model.</typeparam>
        /// <param name="domainEvent">EVent message to apply to the domain object or domain model</param>
        protected void ApplyEvent<TEvent>(TEvent domainEvent) where TEvent : class, IDomainEvent
        {
            Apply(domainEvent, false);
        }

        /// <summary>
        /// This will register an event handler on the aggregate root in which to apply the changes
        /// to the root local state values.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandler"></param>
        protected void RegisterEvent<TEvent>(Action<TEvent> eventHandler) where TEvent : class, IDomainEvent
        {
            _registeredEvents.Add(typeof (TEvent), theEvent => eventHandler(theEvent as TEvent));
        }

        public virtual void Apply<TEvent>(TEvent currentEvent, bool isLoadingFromHistory)
            where TEvent : class, IDomainEvent
        {
            Action<IDomainEvent> handle;
            if (!_registeredEvents.TryGetValue(currentEvent.GetType(), out handle))
                throw new UnregisteredEventHandlerOnAggregateForEventException(GetType(), currentEvent.GetType());

            handle(currentEvent);

            if (!isLoadingFromHistory)
            {
                // update the current event with the information from the aggregate:
                GetVersion();
                PrepareDomainEvent(currentEvent);
                Record(currentEvent);
            }
        }

        protected TEvent PrepareDomainEvent<TEvent>(TEvent domainEvent) where TEvent : class, IDomainEvent
        {
            domainEvent.Version = Version;
            domainEvent.AggregateId = Id;
            domainEvent.EventDateTime = DateTime.Now;

            if (string.IsNullOrEmpty(domainEvent.Who))
                domainEvent.Who = Thread.CurrentPrincipal.Identity.Name;

            return domainEvent;
        }

        protected void GetVersion()
        {
            Version++;
        }
    }
}