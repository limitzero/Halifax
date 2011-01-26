using System;
using Halifax.Commanding;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem
{
    [Serializable]
    public class RemoveItemFromCartCommand : Command
    {
        public Guid CartId { get; set; }
        public string Username { get; set; }
        public string SKU { get; set; }

        public RemoveItemFromCartCommand(Guid cartId, string username, string sku)
        {
            CartId = cartId;
            Username = username;
            SKU = sku;
        }
    }
}