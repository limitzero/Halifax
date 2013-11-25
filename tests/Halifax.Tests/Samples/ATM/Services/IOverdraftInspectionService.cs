using System;

namespace Halifax.Tests.Samples.ATM.Services
{
	/// <summary>
	/// Contract for domain service that will check the current balance of an account
	/// to see if the withdrawal is possible for the given amount.
	/// </summary>
	public interface IOverdraftInspectionService
	{
		bool IsOverdrawn(string accountNumber, decimal withdrawalAmount, out decimal  balance);
	}
}