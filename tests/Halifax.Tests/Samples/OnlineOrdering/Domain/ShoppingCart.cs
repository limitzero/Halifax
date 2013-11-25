using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Domain;
using Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem;
using Halifax.Tests.Samples.OnlineOrdering.Domain.Services;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain
{
	// pure behavioral model with no internal state...just logic (CQRS + ES)
    public class ShoppingCart : AggregateRoot
    {
    	private readonly IItemsInCurrentCartService itemsInCurrentCartService;

		// idea: if we use the domain service to check the duplicate rule, it will not be there as 
		// the event may take some time in propogating to persistance store (be careful here):
        public ShoppingCart(IItemsInCurrentCartService itemsInCurrentCartService)
        {
        	this.itemsInCurrentCartService = itemsInCurrentCartService;
        }

    	public void Create(string username)
        {
        	var experation = System.DateTime.Now.AddMinutes(6);
            var e = new CartCreatedEvent(username, experation);
            Apply(e);
        }

        public void AddItem(string username, string sku, int quantity)
        {
			GuardAgainstDuplicateItems(sku);

			// rule: all child entities created inside the aggregate 
			// must have the identity created within the aggregate
			// that way the consistency of other entities that use the 
			// identity value are in sync:
        	var itemId = CombGuid.NewGuid();

			var e = new ItemAddedToCart(itemId, username, sku, quantity);
            Apply(e);
        }

        public void RemoveItem(Guid itemId, string username, string sku)
        {
            var e = new ItemRemovedFromCart(itemId, username, sku);
            Apply(e);
        }

        private void OnCartCreatedEvent(CartCreatedEvent @event)
        {
        }

        private void OnItemAddedToCartEvent(ItemAddedToCart @event)
        {
            //var item = new ShoppingCartItem(@event.SKU, @event.Quantity);
            //_items.Add(item);
        }

        private void OnItemRemovedFromCartEvent(ItemRemovedFromCart @event)
        {
		
        }

        private void GuardAgainstDuplicateItems(string SKU)
        {
			// use cart service to guard on duplications (this is logic the aggregate root 
			// cannot fulfill on its own...reach out and get some help with it):
			bool isAlreadyInCart = 
        	itemsInCurrentCartService.GetItemsForCurrentShoppingCart(this.Id)
        		.Any(item => item.SKU.Equals(SKU));

			if(isAlreadyInCart == true)
				throw new ItemAlreadyPresentInCartException(SKU);
        }
    }
}