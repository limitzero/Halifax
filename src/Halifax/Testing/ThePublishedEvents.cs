using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Halifax.Commands;
using Halifax.Configuration;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Events;
using Halifax.Internals.Exceptions;

namespace Halifax.Testing
{
    public class ThePublishedEvents : List<Event>
    {
        private readonly Command _command;
        private readonly IContainer _container;
        private Event _event;

        public ThePublishedEvents(IContainer container)
            : this(container, null)
        {
        }

        public ThePublishedEvents(IContainer container, Command command)
        {
            _container = container;
            _command = command;

            ICollection<Event> history = _container.Resolve<IEventStorage>().GetCreationEvents();

            if (history.Count > 0)
            {
                ICollection<Event> events = _container.Resolve<IEventStorage>()
                    .GetHistory(history.First().EventSourceId);
                AddRange(events.Where( ev=> ev !=null).ToList());
            }
        }

        public Event Latest()
        {
            if (Count == 0)
                if (_command != null)
                    throw new NoEventsWerePublishedButSomeWereExpectedException(_command);
                else
                {
                    throw new NoEventsWerePublishedButSomeWereExpectedException();
                }

            _event = this.Last();
            return _event;
        }

        public TEvent Latest<TEvent>() where TEvent : Event
        {
            Event domainEvent = this.Last();
            return domainEvent as TEvent;
        }

    }
}