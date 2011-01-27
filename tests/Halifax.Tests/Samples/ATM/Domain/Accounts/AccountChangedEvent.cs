using System;
using Halifax.Eventing;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts
{
    /// <summary>
    /// Parent event denoting changes to the aggregate.
    /// </summary>
    [Serializable]
    public class AccountChangedEvent : DomainEvent
    {
        /// <summary>
        /// Gets or sets the account number that the event will be recorded for.
        /// </summary>
        public string AccountNumber { get; set; }
    }
}