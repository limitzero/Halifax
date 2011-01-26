using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Halifax.Commanding;
using Halifax.Eventing;
using Halifax.Exceptions;
using Halifax.Storage.Events;

namespace Halifax.Testing
{
    public class ThePublishedEvents : List<IDomainEvent>
    {
        private readonly Command _command;
        private readonly IWindsorContainer _container;
        private IDomainEvent _event;

        public ThePublishedEvents(IWindsorContainer container)
            : this(container, null)
        {
        }

        public ThePublishedEvents(IWindsorContainer container, Command command)
        {
            _container = container;
            _command = command;

            ICollection<IDomainEvent> history = _container.Resolve<IEventStorage>().GetCreationEvents();

            if (history.Count > 0)
            {
                ICollection<IDomainEvent> events = _container.Resolve<IEventStorage>()
                    .GetHistory(history.First().AggregateId);
                AddRange(events);
            }
        }

        public IDomainEvent Latest()
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

        public TEvent Latest<TEvent>() where TEvent : class, IDomainEvent
        {
            IDomainEvent domainEvent = this.Last();
            return domainEvent as TEvent;
        }

    }
}