using System;
using System.Collections.Generic;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.Services
{
	/// <summary>
	/// This is an example of a domain service that "helps" the shopping cart aggregate
	/// to provide the answer to a question it can not get on its own and should remain out 
	/// of the realm of the aggregate.
	/// </summary>
	public interface IItemsInCurrentCartService
	{
		IEnumerable<ReadModel.ShoppingCartItem> GetItemsForCurrentShoppingCart(Guid shoppingCartId);
	}
}