using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Halifax.Eventing;

namespace Halifax.Storage.Internals.Convertor
{
    public class EventConvertor : IEventConvertor
    {
        public IDomainEvent NewEvent { get; private set; }

        public IDomainEvent OldEvent { get; private set; }

        public IDomainEvent Convert(IDomainEvent @event)
        {
            var newEvent = this.MapProperties(@event);
            return newEvent;
        }

        public void RegisterConversion<TNEWEVENT, TOLDEVENT>()
            where TOLDEVENT : class, IDomainEvent, new()
            where TNEWEVENT : class, TOLDEVENT, new()
        {
            this.OldEvent = new TOLDEVENT();
            this.NewEvent = new TNEWEVENT();
        }

        private ICollection<PropertyInfo> GetLocalProperties(IDomainEvent @event)
        {
            var properties =
                (from property in @event.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                 select property).Distinct().ToList();
            return properties;
        }

        private IDomainEvent MapProperties(IDomainEvent @event)
        {
            var newEvent = Activator.CreateInstance(this.NewEvent.GetType()) as IDomainEvent;

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
                catch (Exception e)
                {
                    continue;
                }
            }

            return newEvent;
        }
    }


}