using System;
using Halifax.Eventing;

namespace Halifax.NHibernate.AggregateStorage.Tests.Domain.Products.CreateProducts
{
    [Serializable]
    public class ProductCreatedEvent : Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}