using System;
using Halifax.Bus.Eventing.Async.Transport;

namespace Halifax.Bus.Eventing.Async.Pipeline
{
    public class PipelineManagerPipelineCompletedForSendingEventArgs : EventArgs
    {
        public PipelineManagerPipelineCompletedForSendingEventArgs(ITransportMessage message)
        {
            Message = message;
        }

        public ITransportMessage Message { get; set; }
    }
}