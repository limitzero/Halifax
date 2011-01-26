using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Halifax.Eventing;
using Halifax.Events;
using Halifax.Exceptions;
using Halifax.Storage.Internals.Convertor;

namespace Halifax
{
    /// <summary>
    /// Aggregate root entity that is responsible for coordinating 
    /// a series of changes to the domain and to all supporing entities
    /// from this root context.
    /// </summary>
    [Serializable]
    public abstract class AbstractAggregateRoot :
        IAggregateRoot,
        IEventProvider,
        ISnapshotable
    {
        private static readonly object _convertors_lock = new object();
        private static readonly object _recorded_events_lock = new object();

        [XmlIgnore]
        private readonly List<IDomainEvent> _recordedEvents;

        [XmlIgnore]
        private readonly Dictionary<Type, Action<IDomainEvent>> _registeredEventConvertors;

        [XmlIgnore]
        private readonly Dictionary<Type, Action<IDomainEvent>> _registeredEvents;

        [XmlIgnore]
        private List<IEventConvertor> _convertors;

        protected AbstractAggregateRoot()
        {
            if (_recordedEvents == null)
                _recordedEvents = new List<IDomainEvent>();

            if (_registeredEvents == null)
                _registeredEvents = new Dictionary<Type, Action<IDomainEvent>>();

            if (_registeredEventConvertors == null)
                _registeredEventConvertors = new Dictionary<Type, Action<IDomainEvent>>();

            if (_convertors == null)
            {
                _convertors = new List<IEventConvertor>();
            }

            RegisterEvents();
            RegisterEventConvertors();
        }

        public int EventingThresholdCount { get; set; }

        [XmlIgnore]
        public Func<bool> IsSnapshotPeriodElapsed { get; set; }

        #region IAggregateRoot Members

        public Guid Id { get; set; }
        public int Version { get; set; }

        #endregion

        #region IEventProvider Members

        public void Clear()
        {
            _recordedEvents.Clear();
        }

        public void Record(IDomainEvent domainEvent)
        {
            lock (_recorded_events_lock)
            {
                _recordedEvents.Add(domainEvent);
            }

            ICollection<IDomainEvent> changes = GetChanges();

            if (IsSnapshotPeriodElapsed != null)
            {
                if (IsSnapshotPeriodElapsed())
                    changes = GetChanges();
            }

        }

        public ICollection<IDomainEvent> GetChanges()
        {
            lock (_recorded_events_lock)
            {
                return _recordedEvents.AsReadOnly();
            }
        }

        public void LoadFromHistory(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                if (typeof(AggregateCreatedEvent) == domainEvent.GetType()) continue;
                if (typeof(AggregateSnapshotCreatedEvent) == domainEvent.GetType()) continue;

                domainEvent.AggregateId = Id;
                domainEvent.Version = Version;

                // convert the event upward if loading from history:
                lock (_convertors_lock)
                {
                    var theConvertor = (from convertor in _convertors
                                        where convertor.OldEvent.GetType() == domainEvent.GetType()
                                        select convertor).FirstOrDefault();

                    if (theConvertor != null)
                    {
                        var aConvertedDomainEvent = theConvertor.Convert(domainEvent);
                        Apply(aConvertedDomainEvent, true);
                    }
                    else
                    {
                        Apply(domainEvent, true);
                    }
                }
            }
        }

        #endregion

        #region ISnapshotable Members

        public int GetRecordedEventsCount()
        {
            return _recordedEvents.Count();
        }

        public void TakeSnapshot()
        {
            // update the current event with the information from the aggregate:
            var ev = new AggregateSnapshotCreatedEvent();
            PrepareDomainEvent(ev);
            ev.Root = this;
            Record(ev);
        }

        #endregion

        /// <summary>
        /// This will register all of the events that will cause a change to the aggregate root.
        /// </summary>
        public virtual void RegisterEvents()
        {
        }

        /// <summary>
        /// This will register all of the event convertors for transforming legacy events 
        /// into the most recent structure (i.e. version upsizing).
        /// </summary>
        public virtual void RegisterEventConvertors()
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
            _registeredEvents.Add(typeof(TEvent), theEvent => eventHandler(theEvent as TEvent));
        }

        /// <summary>
        /// This will register an event convertor for the old event for transcribing to the 
        /// new event structure for upgrading (i.e. loading from history). The new event structure 
        /// must inherit from the old striucture in order for the conversion to take place.
        /// </summary>
        /// <typeparam name="TOLDEVENT">The type to convert to a newer representation.</typeparam>
        /// <typeparam name="TNEWEVENT">The type of the newer event</typeparam>
        protected void RegisterConvertor<TOLDEVENT, TNEWEVENT>()
            where TOLDEVENT : class, IDomainEvent, new()
            where TNEWEVENT : class, TOLDEVENT, new()
        {
            // convert the old event to the new event (map avaliable properties, newer properties will not be present):
            lock (_convertors_lock)
            {
                IEventConvertor convertor = new EventConvertor();
                convertor.RegisterConversion<TNEWEVENT, TOLDEVENT>();
                _convertors.Add(convertor);
            }
        }

        public virtual void Apply<TEvent>(TEvent currentEvent, bool isLoadingFromHistory)
            where TEvent : class, IDomainEvent
        {
            // Credit: Mark Nijhof for Fohijin implementation:
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