using System;

namespace Halifax.Domain
{
    /// <summary>
    /// Contract for creating and finding an instance of an aggregate root.
    /// </summary>
    public interface IAggregateRootRepository
    {
		/// <summary>
		/// This will control the creation cycle of the aggregate root 
		/// specifically for setting up the entity identifier and 
		/// instantiating a new instance for use in processing commands.
		/// </summary>
		/// <returns></returns>
		TAggregateRoot Get<TAggregateRoot>() where TAggregateRoot : AggregateRoot;

        /// <summary>
        /// This will control the creation cycle of the aggregate root 
        /// specifically for extracting an existing aggregate root  for use by
        /// re-hydrating the aggregate from the recent set of applied events
        /// where the identifier of a previously activated instance has been created.
        /// </summary>
        /// <returns></returns>
		TAggregateRoot Get<TAggregateRoot>(Guid id) where TAggregateRoot : AggregateRoot;
    }
}