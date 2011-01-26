using Halifax.Eventing;
using Halifax.Tests.Samples.OnlineOrdering.Domain.ReadModel;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem
{
    public class ItemAddedToCartEventConsumer :
        EventConsumer.For<ItemAddedToCartEvent>
    {
        public void Handle(ItemAddedToCartEvent theEvent)
        {
            ReadModelDB.CreateCartItem(theEvent.Username, theEvent.SKU, theEvent.Quantity);
        }
    }
}