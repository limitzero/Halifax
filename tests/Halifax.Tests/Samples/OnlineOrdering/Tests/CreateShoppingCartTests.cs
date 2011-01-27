using Halifax.Testing;
using Halifax.Tests.Samples.OnlineOrdering.Domain;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Xunit;

namespace Halifax.Tests.Samples.OnlineOrdering.Tests
{
    public class when_a_shopping_cart_is_created :
        BaseAggregateWithCommandConsumerTestFixture<ShoppingCart,
            CreateCartCommand,
            CreateCartCommandConsumer>
    {
        private const string _username = "jdoe";

        public override CreateCartCommand When()
        {
            return new CreateCartCommand(_username);
        }

        [Fact]
        public void it_will_publish_an_event_denoting_that_the_cart_was_created()
        {
            PublishedEvents.Latest().WillBeOfType<CartCreatedEvent>();
        }

        [Fact]
        public void it_will_setup_the_cart_to_be_valid_for_five_minutes()
        {
            var now = System.DateTime.Now;
            Assert.Equal(_username, PublishedEvents.Latest<CartCreatedEvent>().Username);
            var duration = PublishedEvents.Latest<CartCreatedEvent>().ValidUntil - now;
            Assert.Equal(5, duration.Minutes);
        }


    }
}