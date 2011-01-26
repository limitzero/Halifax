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
        public string AccountNumber { get; set; }
    }
}