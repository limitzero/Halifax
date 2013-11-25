using System;
using Halifax.Commands;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem
{
    [Serializable]
    public class RemoveItemFromCartCommand : Command
    {
    	public Guid CartId { get; set; }
		public Guid ItemId { get; set; }
        public string Username { get; set; }
        public string SKU { get; set; }

        public RemoveItemFromCartCommand(Guid cartId, Guid itemId, string username, string sku)
        {
        	ItemId = itemId;
        	CartId = cartId;
            Username = username;
            SKU = sku;
        }
    }
}