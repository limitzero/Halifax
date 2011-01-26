using System;
using System.Collections.Generic;
using Halifax.Eventing;
using Halifax.Testing;
using Halifax.Tests.Samples.OnlineOrdering.Domain;
using Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem;
using Xunit;

namespace Halifax.Tests.Samples.OnlineOrdering.Tests
{
    public class when_an_item_is_removed_from_an_existing_cart :
        BaseAggregateWithCommandConsumerTestFixture<ShoppingCart, 
            RemoveItemFromCartCommand,
            RemoveItemFromCartCommandConsumer>
    {
        private const string _sku = "1234567890";
        private const int _quantity = 2;
        private const string _username = "jdoe";

        public override IEnumerable<IDomainEvent> Given()
        {
            yield return new CartCreatedEvent(_username, DateTime.Now);
            yield return new ItemAddedToCartEvent(_username, _sku, _quantity);
        }

        public override RemoveItemFromCartCommand When()
        {
            return new RemoveItemFromCartCommand(Aggregate.Id, _username, _sku);
        }

        [Fact]
        public void it_will_publish_an_event_denoting_that_the_item_was_removed()
        {
            PublishedEvents.Latest().WillBeOfType<ItemRemovedFromCartEvent>();
        }

        [Fact]
        public void it_will_note_within_the_event_that_the_item_for_the_indicated_SKU_was_slated_for_removal()
        {
            PublishedEvents.Latest<ItemRemovedFromCartEvent>().SKU.WillBe(_sku);
        }

     
    }
}