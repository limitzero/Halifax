using Halifax.Testing;
using Halifax.Tests.Samples.OnlineOrdering.Domain;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Machine.Specifications;
using Xunit;

namespace Halifax.Tests.Samples.OnlineOrdering.Tests
{
    public class when_a_shopping_cart_is_created :
        BaseAggregateRootTestFixture<ShoppingCart, CreateCartCommand>
    {
        private const string _username = "jdoe";

        public override CreateCartCommand When()
        {
            return new CreateCartCommand(_username);
        }

		It it_will_publish_an_event_denoting_that_the_cart_was_created = () =>
		{
			PublishedEvents.Latest().WillBeOfType<CartCreatedEvent>();
		};

		It it_will_setup_the_cart_to_be_valid_for_five_minutes = () =>
		{
			var now = System.DateTime.Now;
			var duration = PublishedEvents.Latest<CartCreatedEvent>().ValidUntil - now;

			PublishedEvents.Latest<CartCreatedEvent>().Username.ShouldEqual(_username);
			duration.Minutes.ShouldEqual(5);
		};


    }
}