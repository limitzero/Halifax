using System;
using Halifax.Read;

namespace Halifax.Tests.Samples.OnlineOrdering.ReadModel
{
	public class ShoppingCartItem : IReadModel
	{
		public Guid Id { get; set; }
		public Guid ShoppingCartId { get; set; }
		public string SKU { get; set; }
		public int Quantity { get; set; }
	}
}