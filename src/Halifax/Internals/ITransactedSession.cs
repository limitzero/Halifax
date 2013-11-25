using System;

namespace Halifax.Internals
{
    /// <summary>
    /// Contract for a transacted session started by <seealso cref="IUnitOfWork"/>
    /// </summary>
    public interface ITransactedSession : IDisposable
    {
        /// <summary>
        /// Commits the changes to the aggregate root and publishes any state changes.
        /// </summary>
        void Commit();
    }
}