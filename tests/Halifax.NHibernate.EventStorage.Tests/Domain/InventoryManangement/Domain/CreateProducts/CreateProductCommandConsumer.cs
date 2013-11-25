using System;
using Halifax.Commands;
using Halifax.Domain;

namespace Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain.CreateProducts
{
    public class CreateProductCommandConsumer : 
        CommandConsumer.For<CreateProduct>
    {
        private readonly IAggregateRootRepository root_repository;

        public CreateProductCommandConsumer(IAggregateRootRepository rootRepository)
        {
            root_repository = rootRepository;
        }

        public override AggregateRoot Execute(CreateProduct command)
        {
            var product = root_repository.Get<Product>(Guid.NewGuid());
             product.Create(command.Name, command.Description);
        	return product;
        }
    }
}