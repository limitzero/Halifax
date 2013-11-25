using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Domain;
using Halifax.Events;
using Halifax.Testing;
using Halifax.Tests.Samples.OnlineOrdering.Domain;
using Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem;
using Xunit;

namespace Halifax.Tests.Samples.OnlineOrdering.Tests
{
    public class when_an_item_is_removed_from_an_existing_cart :
        BaseAggregateRootTestFixture<ShoppingCart, RemoveItemFromCartCommand>
    {
        private const string _sku = "1234567890";
        private const int _quantity = 2;
        private const string _username = "jdoe";
    	private Guid item_id;
		
		public override void Initially()
		{
			item_id = CombGuid.NewGuid();
		}

        public override IEnumerable<Event> Given()
        {
            yield return new CartCreatedEvent(_username, DateTime.Now);
            yield return new ItemAddedToCart(item_id,  _username, _sku, _quantity);
        }

        public override RemoveItemFromCartCommand When()
        {
			return new RemoveItemFromCartCommand(AggregateRoot.Id, item_id, _username, _sku);
        }

        [Fact]
        public void it_will_publish_an_event_denoting_that_the_item_was_removed()
        {
            PublishedEvents.Latest().WillBeOfType<ItemRemovedFromCart>();
        }

        [Fact]
        public void it_will_note_within_the_event_that_the_item_for_the_indicated_SKU_was_slated_for_removal()
        {
            PublishedEvents.Latest<ItemRemovedFromCart>().SKU.WillBe(_sku);
        }
    }
}