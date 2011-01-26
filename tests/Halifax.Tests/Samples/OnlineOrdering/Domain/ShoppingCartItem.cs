using System;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain
{
    [Serializable]
    public class ShoppingCartItem
    {
        public string SKU { get; set; }
        public int Quantity { get; set; }

        public ShoppingCartItem(string sku, int quantity)
        {
            SKU = sku;
            Quantity = quantity;
        }
    }
}