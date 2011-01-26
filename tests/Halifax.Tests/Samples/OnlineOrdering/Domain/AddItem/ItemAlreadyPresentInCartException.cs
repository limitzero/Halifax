using System;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem
{
    public class ItemAlreadyPresentInCartException : ApplicationException
    {
        private const string _message = "The item with the SKU of '{0}' has already been added to the cart.";

        public ItemAlreadyPresentInCartException(string sku)
            :base(string.Format(_message, sku))
        {
            
        }
    }
}