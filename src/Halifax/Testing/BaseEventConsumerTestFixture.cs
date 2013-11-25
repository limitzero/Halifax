using System;
using System.Collections.Generic;
using Halifax.Configuration.Impl.Eventing;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Domain;
using Halifax.Events;
using Halifax.Internals.Commands;
using Halifax.Internals.Events;
using Halifax.Internals.Exceptions;
using Halifax.Read;

namespace Halifax.Testing
{
    /// <summary>
    /// Test fixture for invoking an event consumer or series of event consumers for a specific event message and 
    /// optionally extracting information from the read models on examining results from a query.
    /// </summary>
    /// <typeparam name="TEvent">Current event to apply to the system to invoke the consumer(s)</typeparam>
    public abstract class BaseEventConsumerTestFixture<TEvent> : BaseTestConfiguration
        where TEvent : Event
    {
        private ICommandBus command_bus;
        private IEventBus event_bus;

		/// <summary>
		/// Gets or sets the id of the event source that triggered the current event. All historical and the current 
		/// event will be correlated by this identifier:
		/// </summary>
		protected Guid EventSourcedId { get; set; }

        protected BaseEventConsumerTestFixture()
        {
        	EventSourcedId = CombGuid.NewGuid();
            InitializeContext();
            ExecuteContext();
        }

        /// <summary>
        /// (Read-Only). The currently caught exception as captured for 
        /// sending the evemt to the event consumer.
        /// </summary>
        public TheCaughtException CaughtException { get; set; }

        /// <summary>
        /// (Read-Only). The set of stored events that are published
        /// out by the event bus.
        /// </summary>
        public static ThePublishedEvents PublishedEvents { get; private set; }

        /// <summary>
        /// This will set up the test fixture with the initial event consumers
        /// as mentioned in <seealso cref="RegisterEventConsumerOf{TCONSUMER}<>"/>
        /// </summary>
        public virtual void Given()
        {
        }

        /// <summary>
        /// This will set up the initial domain events to be issued 
        /// against the event consumer(s) to initialize the event 
        /// storage.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Event> WithHistoryOf()
        {
            return new List<Event>();
        }

        /// <summary>
        /// This will intialize the context for sending the event to the event consumer.
        /// </summary>
        public abstract TEvent When();

        public virtual void Finally()
        {
        }

		/// <summary>
		/// This will execute a query against the read model and return the result inside of the query object.
		/// </summary>
		/// <typeparam name="TReadModel"></typeparam>
		/// <param name="query"></param>
		protected static void ExecuteQueryOverReadModel<TReadModel>(IQueryExecuter<TReadModel> query) 
			where TReadModel : class, IReadModel
		{
			var read_model_repository =
				Configuration.CurrentContainer()
				.Resolve<IReadModelRepository<TReadModel>>();

			read_model_repository.Query(query);
		}

    	private void ExecuteContext()
        {
            CaughtException =
                new TheCaughtException(new NoExceptionWasCaughtButOneWasExpectedException());

            Given();

            // create the event:
            TEvent @event = When();

            PrepareEvent(@event);

            try
            {
                // set up any historical events for processing:
                MapHistoricalEventsToParentsAndSend(@event);

                // issue the current event against the consumer(s):
                event_bus.Publish(@event);
            }
            catch (Exception e)
            {
                if (typeof(HalifaxException).IsAssignableFrom(e.GetType()))
                    throw e;

                if(e is NotImplementedException)
                    throw e;

                CaughtException = new TheCaughtException(e);
            }
            finally
            {
                Finally();
            }

            PublishedEvents = new ThePublishedEvents(Configuration.CurrentContainer());
        }

        private void PrepareEvent(Event @event)
        {
            // must log the creation event for the fake aggregate
            // to correlate all of the subsequent events against.
			var ce = new AggregateCreatedEvent { EventSourceId = this.EventSourcedId };
            @event.EventSourceId = this.EventSourcedId;
			Configuration.CurrentContainer().Resolve<IEventStorage>().Save(ce);
			Configuration.CurrentContainer().Resolve<IEventStorage>().Save(@event);
        }

        private void MapHistoricalEventsToParentsAndSend(Event parent)
        {
            var history = new List<Event>(WithHistoryOf());

            if (history.Count > 0)
            {
                foreach (Event item in history)
                {
                    item.EventSourceId = parent.EventSourceId;
					Configuration.CurrentContainer().Resolve<IEventStorage>().Save(item);
                    event_bus.Publish(item);
                }
            }
        }

        private void InitializeContext()
        {
        	BuildInfrastructure();

            // start the commading bus for distributing messages to the command handlers:
			command_bus = Configuration.CurrentContainer().Resolve<ICommandBus>();

            // start the eventing bus for distributing messages to the event handlers:
			event_bus = Configuration.CurrentContainer().Resolve<IEventBus>();
        }
    }
}