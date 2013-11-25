using System;
using System.Linq;
using Halifax.Read;

namespace Halifax.Tests.Samples.ATM.ReadModel
{
	public class AccountBalanceQuery : Query<ReadModel.AccountTransaction, decimal >
	{
		private readonly string accountNumber;

		public AccountBalanceQuery(string accountNumber)
		{
			this.accountNumber = accountNumber;
		}

		public override void Execute(IQueryable<ReadModel.AccountTransaction> queryable)
		{
			this.Result = queryable.Where(model => model.AccountNumber.Equals(this.accountNumber))
				.Sum(model => model.Amount);
		}
	}
}