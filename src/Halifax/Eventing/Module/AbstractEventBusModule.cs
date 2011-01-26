using System;

namespace Halifax.Eventing.Module
{
    /// <summary>
    /// Event bus module that will introduce functionality in between the time that 
    /// the message is starting to be published to the event handlers and after 
    /// the message has been published to the event handlers.
    /// </summary>
    public abstract class AbstractEventBusModule : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            OnEventBusMessageModuleDisposing();
        }

        #endregion

        public abstract void OnEventBusStartMessagePublishing(EventBusStartPublishMessageEventArgs args);
        public abstract void OnEventBusCompletedMessagePublishing(EventBusCompletedPublishMessageEventArgs args);

        public virtual void OnEventBusMessageModuleDisposing()
        {
        }
    }
}