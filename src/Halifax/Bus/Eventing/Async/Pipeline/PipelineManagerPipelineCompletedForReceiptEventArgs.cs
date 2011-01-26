using System;
using Halifax.Eventing;

namespace Halifax.Bus.Eventing.Async.Pipeline
{
    public class PipelineManagerPipelineCompletedForReceiptEventArgs : EventArgs
    {
        public PipelineManagerPipelineCompletedForReceiptEventArgs(IDomainEvent @event)
        {
            Event = @event;
        }

        public IDomainEvent Event { get; set; }
    }
}