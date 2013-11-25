using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using Halifax.Commands;
using Halifax.Domain.Internal;
using Halifax.Events;
using Halifax.Internals.Convertor;
using Halifax.Internals.Events;
using Halifax.Read;

namespace Halifax.Domain
{
	/// <summary>
	/// Aggregate root entity that is responsible for coordinating 
	/// a series of changes to the domain and to all supporing entities
	/// from this root context. This is the behavioral model for a root 
	/// entity in collaboration with one or more services/entities.
	/// </summary>
	[Serializable]
	public abstract class AggregateRoot :
		IAggregateRoot,
		IEventProvider,
		ISnapshotable
	{
		private static readonly object convertors_lock = new object();
		private static readonly object recorded_events_lock = new object();

		[XmlIgnore]
		private readonly List<Event> recorded_events;

		[XmlIgnore]
		private readonly Dictionary<Type, Action<Event>> registered_event_convertors;

		[XmlIgnore]
		private readonly Dictionary<Type, Action<Event>> registered_events;

		[XmlIgnore]
		private readonly List<IEventConvertor> convertors;

		[XmlIgnore]
		public int SnapshotLevel { get; set; }

		public Guid Id { get; set; }

		public int Version { get; set; }

		public int EventingThresholdCount { get; set; }

		/// <summary>
		/// Gets or sets the current command that is applied to the aggregate.
		/// </summary>
		public Command CurrentCommand { get; set; }

		/// <summary>
		/// Initializes a new instance of an entity that acts as an aggregate root.
		/// </summary>
		protected AggregateRoot()
		{
			if (recorded_events == null)
				recorded_events = new List<Event>();

			if (registered_events == null)
				registered_events = new Dictionary<Type, Action<Event>>();

			if (registered_event_convertors == null)
				registered_event_convertors = new Dictionary<Type, Action<Event>>();

			if (convertors == null)
			{
				convertors = new List<IEventConvertor>();
			}

			this.SnapshotLevel = 100;

			RegisterEventConvertors();
		}

		public void Clear()
		{
			recorded_events.Clear();
		}

		public void Record(Event domainEvent)
		{
			lock (recorded_events_lock)
			{
				recorded_events.Add(domainEvent);
			}
		}

		public IEnumerable<Event> GetChanges()
		{
			lock (recorded_events_lock)
			{
				return recorded_events.AsReadOnly();
			}
		}

		public void LoadFromHistory(IEnumerable<Event> @events)
		{
			foreach (Event @event in @events)
			{
				if (typeof(AggregateCreatedEvent) == @event.GetType()) continue;
				if (typeof(AggregateSnapshotCreatedEvent) == @event.GetType()) continue;

				@event.EventSourceId = Id;
				@event.Version = Version;

				// convert the event upward if loading from history:
				lock (convertors_lock)
				{
					var theConvertor = (from convertor in convertors
										where convertor.OldEvent.GetType() == @event.GetType()
										select convertor).FirstOrDefault();

					if (theConvertor != null)
					{
						var converted_event = theConvertor.Convert(@event);
						if (converted_event == null) continue;
						this.InvokeApplyMethodForEvent(converted_event);
					}
					else
					{
						this.InvokeApplyMethodForEvent(@event);
					}
				}
			}

			// place the current version of the command to be 
			// the maximum version of the events plus one 
			// indicating that this command "can" update 
			// the state of the aggregate and conflicts can 
			// be done if the command version is less than 
			// the max of version of the stream of events:
			if (@events != null && @events.Count() > 0)
			{
				if (this.CurrentCommand != null)
				{
					this.CurrentCommand.Version = @events.Max(ev => ev.Version) + 1;
				}
			}

		}

		#region ISnapshotable Members

		public int GetRecordedEventsCount()
		{
			return recorded_events.Count();
		}

		public void TakeSnapshot()
		{
			// update the current event with the information from the aggregate:
			var ev = new AggregateSnapshotCreatedEvent();
			PrepareEvent(ev);
			ev.Root = this;
			Record(ev);
		}

		#endregion

		/// <summary>
		/// This will register all of the event convertors for transforming legacy events 
		/// into the most recent structure (i.e. version upsizing).
		/// </summary>
		public virtual void RegisterEventConvertors()
		{
		}

		/// <summary>
		/// This will register an event convertor for the old event for transcribing to the 
		/// new event structure for upgrading (i.e. loading from history). The new event structure 
		/// must inherit from the old striucture in order for the conversion to take place.
		/// </summary>
		/// <typeparam name="TOLDEVENT">The type to convert to a newer representation.</typeparam>
		/// <typeparam name="TNEWEVENT">The type of the newer event</typeparam>
		protected void RegisterConvertor<TOLDEVENT, TNEWEVENT>()
			where TOLDEVENT : Event, new()
			where TNEWEVENT : class, TOLDEVENT, new()
		{
			// convert the old event to the new event (map avaliable properties, newer properties will not be present):
			lock (convertors_lock)
			{
				IEventConvertor convertor = new EventConvertor();
				convertor.RegisterConversion<TNEWEVENT, TOLDEVENT>();
				convertors.Add(convertor);
			}
		}

		/// <summary>
		/// This will apply the given domain event to the domain entity
		/// and subsequently look for the public event method on the 
		/// domain entity to carry out the resulting domain logic (if specified)
		/// for the event.
		/// </summary>
		/// <param name="event">EVent message to apply to the domain object or domain model</param>
		public virtual void Apply(Event @event)
		{
			this.InvokeApplyMethodForEvent(@event);

			bool iseventSourced = typeof(IReadModel).IsAssignableFrom(this.GetType()) == false;

			// check to see if the command that is issued is "behind"
			// the set of events that have transpired to the aggregate:
			if (iseventSourced == true)
				IsConflictingCommandForEvent(@event);

			// update the current event with the information from the aggregate:
			GetVersion();
			PrepareEvent(@event);

			// take a snapshot of the aggregate to mitigate long-history loading
			// for hydration of state:
			if (iseventSourced == true)
			{
				if (GetRecordedEventsCount() > SnapshotLevel)
				{
					this.TakeSnapshot();
				}
			}

			Record(@event);
		}

		/// <summary>
		/// This will prepare the current event for synchronization with the aggregate root.
		/// </summary>
		/// <param name="event"></param>
		/// <returns></returns>
		private Event PrepareEvent(Event @event)
		{
			@event.Version = Version;
			@event.EventSourceId = Id;
			@event.At = DateTime.Now;
			@event.From = this.GetType().FullName;

			if (string.IsNullOrEmpty(@event.Who) == true)
			{
				@event.Who = Thread.CurrentPrincipal.Identity.Name;
			}

			return @event;
		}

		/// <summary>
		/// This will update the current version of the aggregate root to match all corresponding events against.
		/// </summary>
		private void GetVersion()
		{
			Version++;
		}

		private void IsConflictingCommandForEvent(Event @event)
		{
			if (typeof(IReadModel).IsAssignableFrom(this.GetType()) == false)
			{
				//if (this.CurrentCommand.Version < @event.Version)
				//    throw new ConcurrencyException();
			}
		}

		private void InvokeApplyMethodForEvent(Event @event)
		{
			var method = this.GetMethodForEvent(@event);

			if (method != null)
			{
				method.Invoke(this, new object[] { @event });
			}
		}

		private MethodInfo GetMethodForEvent(Event @event)
		{
			MethodInfo method = null;

			method = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
			   .Where(m => m.GetParameters().
							   Where(p => p.ParameterType == @event.GetType()).FirstOrDefault() != null)
			   .Select(m => m).FirstOrDefault();

			return method;
		}
	}
}