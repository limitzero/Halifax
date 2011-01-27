using System;
using Halifax.Eventing;

namespace Halifax.NHibernate.EventStorage.Tests.Domain.Products.CreateProducts
{
    [Serializable]
    public class ProductCreatedEvent : DomainEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}