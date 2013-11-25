using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Halifax.Events;

namespace Halifax.Internals.Convertor
{
    public class EventConvertor : IEventConvertor
    {
        public Event NewEvent { get; private set; }

        public Event OldEvent { get; private set; }

        public Event Convert(Event @event)
        {
            var newEvent = this.MapProperties(@event);
            return newEvent;
        }

        public void RegisterConversion<TNEWEVENT, TOLDEVENT>()
            where TOLDEVENT : Event, new()
            where TNEWEVENT : class, TOLDEVENT, new()
        {
            this.OldEvent = new TOLDEVENT();
            this.NewEvent = new TNEWEVENT();
        }

        private ICollection<PropertyInfo> GetLocalProperties(Event @event)
        {
            var properties =
                (from property in @event.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                 select property).Distinct().ToList();
            return properties;
        }

        private Event MapProperties(Event @event)
        {
            var newEvent = Activator.CreateInstance(this.NewEvent.GetType()) as Event;

            foreach (var property in GetLocalProperties(@event))
            {
                try
                {
                    if (newEvent.GetType().GetProperty(property.Name) != null)
                    {
                        PropertyInfo theProperty = newEvent.GetType().GetProperty(property.Name);
                        theProperty.SetValue(newEvent, property.GetValue(@event, null), null);
                    }
                }
                catch
                {
                    continue;
                }
            }

            return newEvent;
        }
    }


}