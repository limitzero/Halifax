using Axiom.Commanding;
using System;

namespace Axiom.NHibernate.AggregateStorage.Tests.Domain.Products.CreateProducts
{
    [Serializable]
    public class CreateProductCommand : Command
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public CreateProductCommand()
            :this(string.Empty, string.Empty)
        {
            
        }

        public CreateProductCommand(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}