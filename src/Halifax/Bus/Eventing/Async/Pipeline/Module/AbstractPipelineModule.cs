using System;

namespace Halifax.Bus.Eventing.Async.Pipeline.Module
{
    /// <summary>
    /// Base class that allows for functionality to be 
    /// interjected before the message is handled 
    /// on the internal pipeline and after it has 
    /// been handlied on the internal pipeline.
    /// </summary>
    public abstract class AbstractPipelineModule : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            OnPipelineModuleDisposing();
        }

        #endregion

        public abstract void OnPipelineStartedForSend(PipelineManagerPipelineStartedForSendingEventArgs e);

        public abstract void OnPipelineCompletedForSend(PipelineManagerPipelineCompletedForSendingEventArgs e);

        public abstract void OnPipelineStartedForReceipt(PipelineManagerPipelineStartedForReceiptEventArgs e);

        public abstract void OnPipelineCompletedForReceipt(PipelineManagerPipelineCompletedForReceiptEventArgs e);

        public virtual void OnPipelineModuleDisposing()
        {
        }
    }
}