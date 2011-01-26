using System;
using Halifax.Bus.Eventing.Async.Transport;

namespace Halifax.Bus.Eventing.Async.Pipeline
{
    public class PipelineManagerPipelineStartedForReceiptEventArgs : EventArgs
    {
        public PipelineManagerPipelineStartedForReceiptEventArgs(ITransportMessage message)
        {
            Message = message;
        }

        public ITransportMessage Message { get; set; }
    }
}