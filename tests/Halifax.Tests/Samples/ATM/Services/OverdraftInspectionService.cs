using Halifax.Read;
using Halifax.Tests.Samples.ATM.ReadModel;

namespace Halifax.Tests.Samples.ATM.Services
{
	public class OverdraftInspectionService : IOverdraftInspectionService
	{
		private readonly IReadModelRepository<ReadModel.AccountTransaction> repository;

		public OverdraftInspectionService(IReadModelRepository<ReadModel.AccountTransaction> repository)
		{
			this.repository = repository;
		}

		public bool IsOverdrawn(string accountNumber, decimal withdrawalAmount, out decimal balance)
		{
			bool result = false;
			balance = decimal.Zero; 

			AccountBalanceQuery query = new AccountBalanceQuery(accountNumber);
			this.repository.Query(query);
			balance = query.Result;

			if(withdrawalAmount.IsGreaterThan(balance))
			//if (withdrawalAmount > balance)
				result = true;

			return result;
		}
	}
}