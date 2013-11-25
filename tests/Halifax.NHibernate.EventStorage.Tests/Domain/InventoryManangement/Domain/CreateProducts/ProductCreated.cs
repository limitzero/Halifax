using System;
using Halifax.Events;

namespace Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain.CreateProducts
{
    [Serializable]
    public class ProductCreated : Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}