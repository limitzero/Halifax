using System;
using System.Collections.Generic;
using Halifax.Eventing;
using Halifax.Testing;
using Halifax.Tests.Samples.OnlineOrdering.Domain;
using Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Xunit;

namespace Halifax.Tests.Samples.OnlineOrdering.Tests
{
    public class when_an_item_is_added_to_an_existing_cart :
        BaseAggregateWithCommandConsumerTestFixture<ShoppingCart, 
            AddItemToCartCommand, 
            AddItemToCartCommandConsumer>
    {
        private const string _sku = "1234567890";
        private const int _quantity = 1;
        private const string _username = "jdoe";

        public override IEnumerable<IDomainEvent> Given()
        {
            yield return new CartCreatedEvent(_username, DateTime.Now);
        }

        public override AddItemToCartCommand When()
        {
            return new AddItemToCartCommand(Aggregate.Id, _username, _sku, _quantity);
        }

        [Fact]
        public void it_will_publish_an_event_denoting_that_an_item_was_added()
        {
            PublishedEvents.Latest().WillBeOfType<ItemAddedToCartEvent>();
        }

        [Fact]
        public void it_will_record_in_the_published_event_the_SKU_and_quantity_for_the_selected_item()
        {
            PublishedEvents.Latest<ItemAddedToCartEvent>().SKU.WillBe(_sku);
            PublishedEvents.Latest<ItemAddedToCartEvent>().Quantity.WillBe(_quantity);
        }

        [Fact]
        public void it_will_generate_a_message_if_the_same_item_is_already_in_the_cart()
        {
            SendAdditionalCommandOf(new AddItemToCartCommand(Aggregate.Id, _username, _sku, _quantity));
            CaughtException.WillBeOfType<ItemAlreadyPresentInCartException>();
        }


    }
}