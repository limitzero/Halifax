using System;
using System.Collections.Generic;
using Halifax.Events;
using Halifax.Testing;
using Halifax.Tests.Samples.OnlineOrdering.Domain;
using Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Machine.Specifications;

namespace Halifax.Tests.Samples.OnlineOrdering.Tests
{
    public class when_an_item_is_added_to_an_existing_cart :
        BaseAggregateRootTestFixture<ShoppingCart, AddItemToCartCommand>
    {
        private const string _sku = "1234567890";
        private const int _quantity = 1;
        private const string _username = "jdoe";

        public override IEnumerable<Event> Given()
        {
            yield return new CartCreatedEvent(_username, DateTime.Now);
        }

        public override AddItemToCartCommand When()
        {
            return new AddItemToCartCommand(AggregateRoot.Id, _username, _sku, _quantity);
        }

		It it_will_publish_an_event_denoting_that_an_item_was_added = () =>
		{
			PublishedEvents.Latest().WillBeOfType<ItemAddedToCart>();
		};

		It it_will_record_in_the_published_event_the_SKU_and_quantity_for_the_selected_item = () =>
		{
			PublishedEvents.Latest<ItemAddedToCart>().SKU.WillBe(_sku);
			PublishedEvents.Latest<ItemAddedToCart>().Quantity.WillBe(_quantity);
		};

		It it_will_generate_a_message_if_the_same_item_is_already_in_the_cart = () =>
		{
			SendAdditionalCommandOf(new AddItemToCartCommand(AggregateRoot.Id, _username, _sku, _quantity));
			CaughtException.WillBeOfType<ItemAlreadyPresentInCartException>();
		};

		It it_will_generate_an_exception_if_a_quantity_less_than_zero_is_supplied_for_an_item = () =>
		{
			SendAdditionalCommandOf(new AddItemToCartCommand(AggregateRoot.Id, _username, "12345", 0));
			System.Diagnostics.Debug.WriteLine(CaughtException.Message);
			CaughtException.WillBeOfType<InvalidOperationException>();
		};
    }
}