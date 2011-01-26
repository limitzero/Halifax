using Axiom.NHibernate.AggregateStorage.Tests.Domain.Products.CreateProducts;
using System;

namespace Axiom.NHibernate.AggregateStorage.Tests.Domain.Products
{
    [Serializable]
    public class Product : AbstractAggregateRoot
    {
        // local state:
        private string _name;
        private string _description;

        public override void RegisterEvents()
        {
            RegisterEvent<ProductCreatedEvent>(OnProductCreatedEvent);
        }

        public void Create(CreateProductCommand command)
        {
            var ev = new ProductCreatedEvent() {Name = command.Name, Description = command.Description};
            ApplyEvent(ev);
        }

        private void OnProductCreatedEvent(ProductCreatedEvent domainEvent)
        {
            _name = domainEvent.Name;
            _description = domainEvent.Description;
        }
    }
}