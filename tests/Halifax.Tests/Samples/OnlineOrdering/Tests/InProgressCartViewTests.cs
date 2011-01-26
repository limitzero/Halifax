using System;
using System.Collections.Generic;
using Halifax.Eventing;
using Halifax.Testing;
using Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Halifax.Tests.Samples.OnlineOrdering.Domain.ReadModel;
using Xunit;

namespace Halifax.Tests.Samples.OnlineOrdering.Tests
{
    public class when_a_cart_is_currently_created_for_the_user_and_items_have_been_added
        : BaseEventConsumerTestFixture<ItemAddedToCartEvent>
    {
        private const string _sku = "1234567890";
        private const int _quantity = 2;
        private const string _username = "jdoe";

        public override void Given()
        {
            ReadModelDB.Refresh();
            RegisterEventConsumerOf<ItemAddedToCartEventConsumer>();
        }

        public override IEnumerable<IDomainEvent> WithHistoryOf()
        {
            yield return new CartCreatedEvent(_username, DateTime.Now);
            yield return new ItemAddedToCartEvent(_username, _sku, _quantity);
        }

        public override ItemAddedToCartEvent When()
        {
            return new ItemAddedToCartEvent(_username, "11111111", 4);
        }

        [Fact]
        public void it_will_display_the_current_items_in_the_cart_for_the_user_to_review()
        {
            PublishedEvents.Latest().WillBeOfType<ItemAddedToCartEvent>();
            var view = ReadModelDB.GetCurrentCart(_username);
            Assert.Equal(2, view.Count);
        }

     
    }
}