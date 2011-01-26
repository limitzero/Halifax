using System;
using Halifax.Storage.Aggregates;

namespace Halifax.Commanding
{
     /// <summary>
    /// Base class used for all command message consumers.
    /// </summary>
    public abstract class CommandConsumer
    {
        #region Nested type: For

        /// <summary>
        /// Base class used for distinguishing the type 
        /// of command message to consume.
        /// </summary>
        /// <typeparam name="TCOMMAND">Type of the command to consume</typeparam>
        public abstract class For<TCOMMAND>
            where TCOMMAND : Command
        {
            public abstract void Execute(IUnitOfWorkSession session, TCOMMAND theCommand);
        }

        #endregion
    }

  
}