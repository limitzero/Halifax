using System;
using Halifax.Read;

namespace Halifax.Tests.Samples.ATM.ReadModel
{
	public class AccountTransaction: IReadModel
	{
		public Guid Id { get; set; }
		public string AccountNumber { get; set; }
		public DateTime At { get; set; }
		public decimal Amount { get; set; }
	}
}