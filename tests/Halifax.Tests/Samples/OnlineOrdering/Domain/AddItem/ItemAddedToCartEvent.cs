using System;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem
{
    [Serializable]
    public class ItemAddedToCart : ShoppingCartItemChanged
    {
		public string SKU { get; set; }
		public int Quantity { get; set; }

        public ItemAddedToCart(Guid id,  string username, string sku, int quantity)
        {
        	ItemId = id;
            UserName = username;
            SKU = sku;
            Quantity = quantity;
        }
    }
}