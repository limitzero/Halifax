using System;

namespace Halifax.Bus.Eventing.Async.Pipeline
{
    public class PipelineManagerPipelineStartedForSendingEventArgs : EventArgs
    {
        public PipelineManagerPipelineStartedForSendingEventArgs(object message)
        {
            Message = message;
        }

        public object Message { get; set; }
    }
}