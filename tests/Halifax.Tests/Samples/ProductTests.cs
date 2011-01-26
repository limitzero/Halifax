using System;
using System.Collections.Generic;
using Halifax.Commanding;
using Halifax.Eventing;
using Halifax.Storage.Aggregates;
using Halifax.Storage.Events;
using Halifax.Testing;
using Xunit;

namespace Halifax.Tests.Samples
{
    public class when_creating_a_product :
        BaseAggregateWithCommandConsumerTestFixture<Product, 
            CreateProductCommand, 
            CreateProductCommandConsumer>
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
        : BaseAggregateWithCommandConsumerTestFixture<Product, 
              ChangeProductPriceCommand, 
              ChangeProductPriceCommandConsumer>
    {
        public override IEnumerable<IDomainEvent> Given()
        {
            // the product has been created:
            yield return new ProductCreatedEvent("Windex", "All-Purpose Cleaner", 15.00M);                    
        }

        public override ChangeProductPriceCommand When()
        {
            return new ChangeProductPriceCommand(Aggregate.Id, 50.00M);
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
        DomainEvent,
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
        DomainEvent, 
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
        private readonly IDomainRepository _repository;

        public CreateProductCommandConsumer(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWorkSession session, CreateProductCommand command)
        {
            var root = _repository.Create<Product>();
            root.CreateProduct(command);
            session.Accept(root);
        }

    }

    public class ChangeProductPriceCommandConsumer :
        CommandConsumer.For<ChangeProductPriceCommand>
    {
        private readonly IDomainRepository _repository;

        public ChangeProductPriceCommandConsumer(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWorkSession session, ChangeProductPriceCommand command)
        {
            // can use ancillary services here to validate the product 
            // price via DI if needed:

            var root = _repository.Find<Product>(command.Id);
            root.ChangePrice(command);
            session.Accept(root);
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

    public class Product : AbstractAggregateRoot
    {
        #region -- local state --
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _price = 0.0M;
        #endregion

        public override void RegisterEvents()
        {
            RegisterEvent<ProductCreatedEvent>(OnProductCreatedEvent);
            RegisterEvent<ProductPriceChangedEvent>(OnProductPriceChangedEvent);
        }

        public void CreateProduct(CreateProductCommand command)
        {
            var ev = new ProductCreatedEvent(command.Name, command.Description, command.Price);
            ApplyEvent(ev);
        }

        public void ChangePrice(ChangeProductPriceCommand command)
        {
            var ev = new ProductPriceChangedEvent(command.NewPrice);
            ApplyEvent(ev);
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