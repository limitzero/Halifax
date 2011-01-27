using System.Transactions;
namespace Halifax
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
        /// (Read-Write). The current command that generated the UoW session.
        /// </summary>
        object CurrentCommand { get; set; }

        /// <summary>
        /// This will start the session for persisting 
        /// changes for the given aggregate root.
        /// </summary>
        /// <param name="root">Aggregate root where changes have been made</param>
        void Accept(AbstractAggregateRoot root);

        /// <summary>
        /// This will start a transaction for recording and storing state
        /// change information to the current aggregate root.
        /// </summary>
        /// <param name="root">Current aggregate root to record changes against.</param>
        /// <returns></returns>
        ITransactedSession BeginTransaction(AbstractAggregateRoot root);

        /// <summary>
        /// This will start a transaction for recording and storing state
        /// change information to the current aggregate root.
        /// </summary>
        /// <param name="root">Current aggregate root to record changes against.</param>
        /// <param name="option">Level of the transaction granularity for the operations.</param>
        /// <returns></returns>
        ITransactedSession BeginTransaction(AbstractAggregateRoot root, TransactionScopeOption option);
    }
}