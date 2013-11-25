using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Read;

namespace Halifax.NHibernate.Tests.Domain.InventoryManangement.ReadModel
{
	public class AllProductsQuery : Query<ReadModel.Product, IEnumerable<ReadModel.Product>>
	{
		public override void Execute(IQueryable<Product> queryable)
		{
			this.Result = queryable.ToList();
		}
	}
}