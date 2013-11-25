using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Read;

namespace Halifax.Tests.Samples.OnlineOrdering.ReadModel
{
	public class ItemsInCurrentCartQuery : Query<ReadModel.ShoppingCartItem, IEnumerable<ShoppingCartItem>>
	{
		private readonly Guid shopping_cart_id;

		public ItemsInCurrentCartQuery(Guid shoppingCartId)
		{
			shopping_cart_id = shoppingCartId;
		}

		public override void Execute(IQueryable<ShoppingCartItem> queryable)
		{
			this.Result = queryable.Where(item => item.ShoppingCartId.Equals(this.shopping_cart_id)).ToList();
		}
	}
}