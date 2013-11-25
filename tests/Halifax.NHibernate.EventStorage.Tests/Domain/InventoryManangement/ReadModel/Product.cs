using System;
using Halifax.Read;

namespace Halifax.NHibernate.Tests.Domain.InventoryManangement.ReadModel
{
	public class Product : IReadModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}