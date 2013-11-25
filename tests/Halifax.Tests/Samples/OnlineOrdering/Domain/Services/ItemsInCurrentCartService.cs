using System;
using System.Collections.Generic;
using Halifax.Read;
using Halifax.Tests.Samples.OnlineOrdering.ReadModel;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.Services
{
	public class ItemsInCurrentCartService : IItemsInCurrentCartService
	{
		private readonly IReadModelRepository<ReadModel.ShoppingCartItem> repository;

		public ItemsInCurrentCartService(IReadModelRepository<ReadModel.ShoppingCartItem> repository)
		{
			this.repository = repository;
		}
		
		public IEnumerable<ReadModel.ShoppingCartItem> GetItemsForCurrentShoppingCart(Guid shoppingCartId)
		{
			var query = new ItemsInCurrentCartQuery(shoppingCartId);
			this.repository.Query(query);
			return query.Result;
		}
	}
}