using Halifax.Commands;
using Halifax.Domain;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem
{
	public class AddItemToCartCommandConsumer
		: CommandConsumer.For<AddItemToCartCommand>
	{
		private readonly IAggregateRootRepository repository;

		public AddItemToCartCommandConsumer(IAggregateRootRepository repository)
		{
			this.repository = repository;
		}

		public override AggregateRoot Execute(AddItemToCartCommand command)
		{
			var cart = repository.Get<ShoppingCart>(command.CartId);
			cart.AddItem(command.Username, command.SKU, command.Quantity);
			return cart;
		}
	}
}