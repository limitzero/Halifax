using System;
using System.Collections.Generic;
using Halifax.Commands;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Domain;
using Halifax.Events;
using Halifax.Testing;
using Xunit;

namespace Halifax.Tests.Samples
{
	public class when_creating_a_product : BaseAggregateRootTestFixture<Product,CreateProductCommand>
	{
		public override CreateProductCommand When()
		{
			return new CreateProductCommand("Windex", "All-purpose cleaner", 5.00M);
		}

		[Fact]
		public void then_a_product_created_event_will_be_published()
		{
			PublishedEvents.Latest().WillBeOfType<ProductCreatedEvent>();
		}

		[Fact]
		public void then_the_published_event_will_contain_the_name_and_description_of_the_product()
		{
			PublishedEvents.Latest<ProductCreatedEvent>().Name.WillBe("Windex");
			PublishedEvents.Latest<ProductCreatedEvent>().Description.WillBe("All-purpose cleaner");
		}
	}

	public class when_changing_the_price_of_a_product
		: BaseAggregateRootTestFixture<Product,
			  ChangeProductPriceCommand>
	{
		public override IEnumerable<Event> Given()
		{
			// the product has been created:
			yield return new ProductCreatedEvent("Windex", "All-Purpose Cleaner", 15.00M);
		}

		public override ChangeProductPriceCommand When()
		{
			return new ChangeProductPriceCommand(AggregateRoot.Id, 50.00M);
		}

		[Fact]
		public void then_a_product_price_changed_event_will_be_published()
		{
			PublishedEvents.Latest().WillBeOfType<ProductPriceChangedEvent>();
		}

		[Fact]
		public void then_the_product_price_will_be_changed_to_the_new_price()
		{
			PublishedEvents.Latest<ProductPriceChangedEvent>().NewPrice.WillBe(50.00M);
		}
	}

	public interface ICreateProductCommandMetaData
	{
		// information for the created product:
		string Name { get; set; }
		string Description { get; set; }
		decimal Price { get; set; }
	}

	public interface IChangeProductPriceCommandMetaData
	{
		Guid Id { get; set; }
		decimal NewPrice { get; set; }
	}

	[Serializable]
	public class CreateProductCommand : Command,
										ICreateProductCommandMetaData
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }

		public CreateProductCommand()
		{
		}

		public CreateProductCommand(string name, string description, decimal price)
		{
			Name = name;
			Description = description;
			Price = price;
		}

	}

	[Serializable]
	public class ChangeProductPriceCommand :
		Command,
		IChangeProductPriceCommandMetaData
	{
		public Guid Id { get; set; }
		public decimal NewPrice { get; set; }

		private ChangeProductPriceCommand()
		{
		}

		public ChangeProductPriceCommand(Guid productId, decimal newPrice)
		{
			Id = productId;
			NewPrice = newPrice;
		}
	}

	[Serializable]
	public class ProductPriceChangedEvent :
		Event,
		IChangeProductPriceCommandMetaData
	{
		// price changed meta-data:
		public Guid Id { get; set; }
		public decimal NewPrice { get; set; }

		private ProductPriceChangedEvent()
		{
		}

		public ProductPriceChangedEvent(decimal newPrice)
		{
			NewPrice = newPrice;
		}
	}

	[Serializable]
	public class ProductCreatedEvent :
		Event,
		ICreateProductCommandMetaData
	{
		// information for the created product:
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }

		private ProductCreatedEvent()
		{
		}

		public ProductCreatedEvent(string name, string description, decimal price)
		{
			Name = name;
			Description = description;
			Price = price;
		}
	}

	public class CreateProductCommandConsumer :
		CommandConsumer.For<CreateProductCommand>
	{
		private readonly IAggregateRootRepository repository;

		public CreateProductCommandConsumer(IAggregateRootRepository repository)
		{
			this.repository = repository;
		}

		public override AggregateRoot Execute(CreateProductCommand command)
		{
			var product = repository.Get<Product>(CombGuid.NewGuid());
			product.CreateProduct(command);
			return product;
		}

	}

	public class ChangeProductPriceCommandConsumer :
		CommandConsumer.For<ChangeProductPriceCommand>
	{
		private readonly IAggregateRootRepository repository;

		public ChangeProductPriceCommandConsumer(IAggregateRootRepository repository)
		{
			this.repository = repository;
		}

		public override AggregateRoot Execute(ChangeProductPriceCommand command)
		{
			var product = repository.Get<Product>(command.Id);
			product.ChangePrice(command);
			return product;
		}
	}

	public class ProductEventHandler
		: EventConsumer.For<ProductCreatedEvent>,
		  EventConsumer.For<ProductPriceChangedEvent>
	{
		private readonly IEventStorage _storage;

		public ProductEventHandler(IEventStorage storage)
		{
			_storage = storage;
		}

		public void Handle(ProductCreatedEvent @event)
		{
			_storage.Save(@event);
		}

		public void Handle(ProductPriceChangedEvent @event)
		{
			_storage.Save(@event);
		}
	}

	public class Product : AggregateRoot
	{
		#region -- local state --
		private string _name = string.Empty;
		private string _description = string.Empty;
		private decimal _price = 0.0M;
		#endregion

		public void CreateProduct(CreateProductCommand command)
		{
			var ev = new ProductCreatedEvent(command.Name, command.Description, command.Price);
			Apply(ev);
		}

		public void ChangePrice(ChangeProductPriceCommand command)
		{
			var ev = new ProductPriceChangedEvent(command.NewPrice);
			Apply(ev);
		}

		private void OnProductCreatedEvent(ProductCreatedEvent domainEvent)
		{
			_name = domainEvent.Name;
			_description = domainEvent.Description;
			_price = domainEvent.Price;
		}

		private void OnProductPriceChangedEvent(ProductPriceChangedEvent domainEvent)
		{
			_price = domainEvent.NewPrice;
		}
	}
}