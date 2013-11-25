using System;
using Halifax.Events;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts
{
    /// <summary>
    /// Parent event denoting changes to the aggregate.
    /// </summary>
    [Serializable]
    public class AccountChanged : Event
    {
        /// <summary>
        /// Gets or sets the account number that the event will be recorded for.
        /// </summary>
        public string AccountNumber { get; set; }

		/// <summary>
		/// Gets or sets the current balance of the account
		/// </summary>
		public decimal Balance { get; set; }
    }
}