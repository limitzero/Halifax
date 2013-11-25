using System.Transactions;
using Halifax.Domain;

namespace Halifax.Internals
{
    /// <summary>
    /// Contract for starting the unit of work (UoW) session 
    /// for persisting all of the changes that were 
    /// made against the aggregate root for a given
    /// command.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// This will start the session for persisting 
        /// changes for the given aggregate root.
        /// </summary>
        /// <param name="root">Aggregate root where changes have been made</param>
        void Accept(AggregateRoot root);
    }
}