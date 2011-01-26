using System;

namespace Halifax.Eventing
{
    /// <summary>
    /// Contract for all aggregate events that are recording for aggregate mutation or inquiry.
    /// </summary>
    public interface IDomainEvent : IMessage
    {
        /// <summary>
        /// (Read-Write). The identifier of the current aggregate root generating the event.
        /// </summary>
        Guid AggregateId { get; set; }

        /// <summary>
        /// (Read-Write). The current version of the aggregate root that has been affected by the event.
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// (Read-Write). The current identity of the calling client that issued the domain change resulting in the event.
        /// </summary>
        string Who { get; set; }

        /// <summary>
        /// (Read-Write). The date and time the event was recorded on the domain aggregate root.
        /// </summary>
        DateTime? EventDateTime { get; set; }
    }
}