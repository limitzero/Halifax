using System;
using Halifax.Eventing;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem
{
    [Serializable]
    public class ItemRemovedFromCartEvent : DomainEvent
    {
        public string Username { get; set; }
        public string SKU { get; set; }
        public DateTime Timestamp { get; set; }

        public ItemRemovedFromCartEvent(string username, string sku, DateTime timestamp)
        {
            Username = username;
            SKU = sku;
            Timestamp = timestamp;
        }
    }
}