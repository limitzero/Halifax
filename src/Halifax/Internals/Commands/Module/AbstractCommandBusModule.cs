using System;

namespace Halifax.Internals.Commands.Module
{
    /// <summary>
    /// Command bus module that will introduce functionality in between the time that 
    /// the message is starting to be sent to the command handlers and after 
    /// the message has been sent to the command handlers.
    /// </summary>
    public abstract class AbstractCommandBusModule : IDisposable
    {
        #region IDisposable Members

        public virtual void Dispose()
        {
            OnCommandBusModuleDisposing();
        }

        #endregion

        public abstract void OnCommandBusStartMessagePublishing(CommandBusStartPublishMessageEventArgs args);
        public abstract void OnCommandBusCompletedMessagePublishing(CommndBusCompletedPublishMessageEventArgs args);

        public virtual void OnCommandBusModuleDisposing()
        {
        }
    }
}