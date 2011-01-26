using System;
using System.Collections.Generic;
using Halifax.Eventing;

namespace Halifax.Storage.Events
{
    /// <summary>
    /// Contract that all storage implementations will adhere to 
    /// for storing events that originate from the domain model.
    /// </summary>
    public interface IEventStorage
    {
        /// <summary>
        /// This will save a domain event to the persistable storage for events.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="domainEvent">Current event messge to persist</param>
        void Save<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;

        /// <summary>
        /// This will retreive the history of all the events for a particular aggregate root
        /// </summary>
        /// <param name="aggregateRootId">Identifier of the aggregate root instance where the events were applied.</param>
        /// <returns></returns>
        ICollection<IDomainEvent> GetHistory(Guid aggregateRootId);

        /// <summary>
        /// This will retreive the history of all the events for a particular aggregate root
        /// </summary>
        /// <param name="aggregateRootId">Identifier of the aggregate root instance where the events were applied.</param>
        /// <returns></returns>
        ICollection<IDomainEvent> GetHistorySinceSnapshot(Guid aggregateRootId);

        /// <summary>
        /// This will return the list of all instances of the creation of an aggregate root.
        /// </summary>
        /// <returns></returns>
        ICollection<IDomainEvent> GetCreationEvents();

        //IDomainEvent[] LoadHistory<TAggregateRoot>() where TAggregateRoot : AbstractAggregateRoot;
    }
}