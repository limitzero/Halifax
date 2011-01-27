using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem;
using Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart;
using Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain
{
    public class ShoppingCart : AbstractAggregateRootByConvention
    {
        #region -- local state --
        private string _username;
        private DateTime _validUntil;
        private readonly List<ShoppingCartItem> _items;
        #endregion

        public ShoppingCart()
        {
            _items = new List<ShoppingCartItem>();
        }

        public void Create(string username)
        {
            var e = new CartCreatedEvent(username, System.DateTime.Now.AddMinutes(5));
            ApplyEvent(e);
        }

        public void AddItem(string username, string sku, int quantity)
        {
            var e = new ItemAddedToCartEvent(username, sku, quantity);
            ApplyEvent(e);
        }

        public void RemoveItem(string sku)
        {
            var e = new ItemRemovedFromCartEvent(_username, sku, DateTime.Now);
            ApplyEvent(e);
        }

        private void OnCartCreatedEvent(CartCreatedEvent @event)
        {
            _username = @event.Username;
            _validUntil = @event.ValidUntil;
        }

        private void OnItemAddedToCartEvent(ItemAddedToCartEvent @event)
        {
            GuardAgainstDuplicateItems(@event.SKU);

            var item = new ShoppingCartItem(@event.SKU, @event.Quantity);
            _items.Add(item);
        }

        private void OnItemRemovedFromCartEvent(ItemRemovedFromCartEvent @event)
        {
            var toRemove = (from item in _items
                            where item.SKU == @event.SKU
                            select item).FirstOrDefault();

            if (toRemove != null)
            {
                _items.Remove(toRemove);
            }
        }

        private void GuardAgainstDuplicateItems(string SKU)
        {
            var item = (from itm in _items
                        where itm.SKU == SKU
                        select itm).FirstOrDefault();

            if(item != null)
                throw new ItemAlreadyPresentInCartException(SKU);
        }
    }
}