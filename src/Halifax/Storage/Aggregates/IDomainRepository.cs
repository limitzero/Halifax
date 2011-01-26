using System;

namespace Halifax.Storage.Aggregates
{
    /// <summary>
    /// Contract for creating and finding an instance of an aggregate root.
    /// </summary>
    public interface IDomainRepository
    {
        /// <summary>
        /// This will control the creation cycle of the aggregate root 
        /// specifically for setting up the entity identifier and 
        /// instatiating a new instance for use.
        /// </summary>
        /// <returns></returns>
        TEntity Create<TEntity>() where TEntity : AbstractAggregateRoot, new();

        /// <summary>
        /// This will find a domain aggregate root by identifier and 
        /// reconstitute the state of the aggregate from the events 
        /// within the event store.
        /// </summary>
        /// <param name="Id">Identifier of the aggregate root.</param>
        /// <returns></returns>
        TEntity Find<TEntity>(Guid Id) where TEntity : AbstractAggregateRoot;
    }
}