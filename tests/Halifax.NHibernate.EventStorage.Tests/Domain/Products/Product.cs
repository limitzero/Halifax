using System;
using Halifax.NHibernate.EventStorage.Tests.Domain.Products.CreateProducts;

namespace Halifax.NHibernate.EventStorage.Tests.Domain.Products
{
    [Serializable]
    public class Product : AbstractAggregateRootByConvention
    {
        // local state:
        private string _name;
        private string _description;

        public override void RegisterEvents()
        {
            // RegisterEvent<ProductCreatedEvent>(OnProductCreatedEvent);
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