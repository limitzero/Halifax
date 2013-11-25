using System;
using Halifax.Domain;
using Halifax.Events;
using Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain.CreateProducts;
using Halifax.Read;

namespace Halifax.NHibernate.Tests.Domain.InventoryManangement.ReadModel
{
	public class InventoryManagementEventConsumer
		: EventConsumer.For<ProductCreated>
	{
		private readonly IReadModelRepository<Product> repository;

		public InventoryManagementEventConsumer(IReadModelRepository<Product> repository)
		{
			this.repository = repository;
		}

		public void Handle(ProductCreated @event)
		{
			repository.Insert(new Product()
			                  	{
			                  		Id = CombGuid.NewGuid(), 
									Description = @event.Description,
									Name =  @event.Name
			                  	});
		}
	}
}