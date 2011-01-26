using Axiom.Eventing;
using System;
namespace Axiom.NHibernate.AggregateStorage.Tests.Domain.Products.CreateProducts
{
    [Serializable]
    public class ProductCreatedEvent : DomainEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}