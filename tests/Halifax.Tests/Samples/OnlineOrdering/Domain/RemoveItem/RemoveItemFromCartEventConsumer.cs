using System;
using Halifax.Events;
using Halifax.Read;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem
{
	public class RemoveItemFromCartEventConsumer :
		EventConsumer.For<ItemRemovedFromCart>
	{
		private readonly IReadModelRepository<ReadModel.ShoppingCartItem> repository;

		public RemoveItemFromCartEventConsumer(IReadModelRepository<ReadModel.ShoppingCartItem> repository)
		{
			this.repository = repository;
		}

		public void Handle(ItemRemovedFromCart @event)
		{
			ReadModel.ShoppingCartItem item = repository.Get(@event.ItemId);

			if (item != null)
			{
				repository.Delete(item);
			}
		}
	}
}