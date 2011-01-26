using System;
using Halifax.Eventing;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem
{
    [Serializable]
    public class ItemAddedToCartEvent : DomainEvent
    {
        public ItemAddedToCartEvent(string username, string sku, int quantity)
        {
            Username = username;
            SKU = sku;
            Quantity = quantity;
        }

        public string Username { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }


    }
}