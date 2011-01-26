using System;
using Halifax.Bus.Eventing.Async.Subscriptions;
using Halifax.Bus.Eventing.Async.Transport;
using Halifax.Eventing;

namespace Halifax.Bus.Eventing.Async.Pipeline
{
    public interface IPipelineManager
    {
        event Action<PipelineManagerPipelineStartedForSendingEventArgs> PipelineStartedForSendEvent;
        event Action<PipelineManagerPipelineCompletedForSendingEventArgs> PipelineCompletedForSendEvent;
        event Action<PipelineManagerPipelineStartedForReceiptEventArgs> PipelineStartedForReceiveEvent;
        event Action<PipelineManagerPipelineCompletedForReceiptEventArgs> PipelineCompletedForReceiveEvent;

        void InvokeForSend(ISubscription[] subscriptions, object message, ITransport transport);
        IDomainEvent InvokeForReceive(ITransportMessage message);
    }
}