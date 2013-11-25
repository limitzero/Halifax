using System;
using Halifax.Commands;

namespace Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain.CreateProducts
{
    [Serializable]
    public class CreateProduct : Command
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public CreateProduct()
            :this(string.Empty, string.Empty)
        {
            
        }

        public CreateProduct(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}