using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Read;

namespace Halifax.Tests.Samples.ATM.ReadModel
{
	public class AccountTransactionHistoryQuery : 
		Query<ReadModel.AccountTransaction, IEnumerable<ReadModel.AccountTransaction>>
	{
		private readonly string accountNumber;
		
		public AccountTransactionHistoryQuery(string accountNumber)
		{
			this.accountNumber = accountNumber;
		}

		public override void Execute(IQueryable<AccountTransaction> queryable)
		{
			this.Result = queryable.Where(txn => txn.AccountNumber.Equals(this.accountNumber));
		}
	}
}