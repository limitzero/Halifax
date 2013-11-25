using System;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem
{
    [Serializable]
    public class ItemRemovedFromCart : ShoppingCartItemChanged
    {
        public string Username { get; set; }
        public string SKU { get; set; }

        public ItemRemovedFromCart(Guid itemId, string username, string sku)
        {
        	this.ItemId = itemId; 
            Username = username;
            SKU = sku;
        }
    }
}