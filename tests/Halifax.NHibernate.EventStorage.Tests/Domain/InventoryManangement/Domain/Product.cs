using Halifax.Domain;
using Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain.CreateProducts;

namespace Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain
{
    public class Product : AggregateRoot
    {
		public void Create(string name, string description)
		{
			var ev = new ProductCreated()
			{
				Name = name,
				Description = description
			};
			Apply(ev);
		}

        private void OnProductCreatedEvent(ProductCreated domainEvent)
        {
           
        }
    }
}