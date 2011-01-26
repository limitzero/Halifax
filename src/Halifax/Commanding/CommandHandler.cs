using System;

namespace Halifax.Commanding
{
    /// <summary>
    /// Base class for handling a single command that will cause 
    /// a possible state change to a domain aggregate.
    /// </summary>
    /// <typeparam name="TCommand">Command to send to the domain aggregate</typeparam>
    [Obsolete]
    public abstract class CommandHandler<TCommand>
        where TCommand : class
    {
        public abstract void Execute(TCommand command);
    }
}