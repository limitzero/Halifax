namespace Halifax
{
    /// <summary>
    /// Contract for starting the unit of work (UoW) session 
    /// for persisting all of the changes that were 
    /// made against the aggregate root for a given
    /// command.
    /// </summary>
    public interface IUnitOfWorkSession
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
    }
}