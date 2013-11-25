using System;
using System.Collections.Generic;
using Halifax.Events;
using Halifax.Testing;
using Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Halifax.Tests.Samples.OnlineOrdering.ReadModel;
using Xunit;

namespace Halifax.Tests.Samples.OnlineOrdering.Tests
{
    public class when_a_cart_is_currently_created_for_the_user_and_items_have_been_added
        : BaseEventConsumerTestFixture<ItemAddedToCart>
    {
        private const string _sku = "1234567890";
        private const int _quantity = 2;
        private const string _username = "jdoe";
		private Guid first_item_id = Guid.NewGuid();
		private Guid second_item_id = Guid.NewGuid();

        public override IEnumerable<Event> WithHistoryOf()
        {
            yield return new CartCreatedEvent(_username, DateTime.Now);
			yield return new ItemAddedToCart(first_item_id, _username, _sku, _quantity);
        }

        public override ItemAddedToCart When()
        {
			return new ItemAddedToCart(second_item_id, _username, "11111111", 4);
        }

        [Fact]
        public void it_will_display_the_current_items_in_the_cart_for_the_user_to_review()
        {
            PublishedEvents.Latest().WillBeOfType<ItemAddedToCart>();

        	var query = new ItemsInCurrentCartCountQuery(this.EventSourcedId); 
			ExecuteQueryOverReadModel(query);

			Assert.Equal(2, query.Result);
        }

     
    }
}