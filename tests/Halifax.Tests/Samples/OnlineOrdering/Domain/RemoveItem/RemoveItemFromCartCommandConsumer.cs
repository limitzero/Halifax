using Halifax.Commands;
using Halifax.Domain;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem
{
    public class RemoveItemFromCartCommandConsumer : 
        CommandConsumer.For<RemoveItemFromCartCommand>
    {
        private readonly IAggregateRootRepository repository;

        public RemoveItemFromCartCommandConsumer(IAggregateRootRepository repository)
        {
            this.repository = repository;
        }

        public override AggregateRoot Execute(RemoveItemFromCartCommand command)
        {
            var cart = this.repository.Get<ShoppingCart>(command.CartId);
             cart.RemoveItem(command.ItemId, command.Username, command.SKU);
        	return cart;
        }
    }
}