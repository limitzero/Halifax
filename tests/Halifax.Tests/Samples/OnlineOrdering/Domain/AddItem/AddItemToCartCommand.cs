using System;
using Halifax.Commanding;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem
{
    [Serializable]
    public class AddItemToCartCommand : Command
    {
        public Guid CartId { get; set; }
        public string Username { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }

        public AddItemToCartCommand(Guid cartId, string username, string sku, int quantity)
        {
            CartId = cartId;
            Username = username;
            SKU = sku;
            Quantity = quantity;
        }
    }
}