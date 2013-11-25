using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Read;

namespace Halifax.Tests.Samples.ATM.ReadModel
{
	public class AllAccountsQuery : Query<ReadModel.Account, IEnumerable<ReadModel.Account>>
	{
		public override void Execute(IQueryable<Account> queryable)
		{
			this.Result = queryable.ToList();
		}
	}
}