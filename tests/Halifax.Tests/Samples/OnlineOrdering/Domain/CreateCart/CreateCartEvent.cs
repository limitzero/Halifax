using System;
using Halifax.Eventing;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart
{
    [Serializable]
    public class CartCreatedEvent : DomainEvent
    {
        public string Username { get; set; }
        public DateTime ValidUntil { get; set; }

        public CartCreatedEvent()
        {
            
        }

        public CartCreatedEvent(string username, DateTime validUntil)
        {
            Username = username;
            ValidUntil = validUntil;
        }

    }
}