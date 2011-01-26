using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Halifax.Bus.Eventing.Async.Pipeline.Module;
using Halifax.Bus.Eventing.Async.Subscriptions;
using Halifax.Bus.Eventing.Async.Transport;
using Halifax.Eventing;
using Halifax.Storage.Internals.Serialization;

namespace Halifax.Bus.Eventing.Async.Pipeline
{
    public class PipelineManager : IPipelineManager
    {
        private readonly IKernel _kernel;
        private readonly ISerializationProvider _serializer;

        public event Action<PipelineManagerPipelineStartedForSendingEventArgs> PipelineStartedForSendEvent;
        public event Action<PipelineManagerPipelineCompletedForSendingEventArgs> PipelineCompletedForSendEvent;
        public event Action<PipelineManagerPipelineStartedForReceiptEventArgs> PipelineStartedForReceiveEvent;
        public event Action<PipelineManagerPipelineCompletedForReceiptEventArgs> PipelineCompletedForReceiveEvent;

        public PipelineManager(IKernel kernel,
                               ISerializationProvider serializer)
        {
            _kernel = kernel;
            _serializer = serializer;
        }

        public void InvokeForSend(ISubscription[] subscriptions, object message, ITransport transport)
        {
            ITransportMessage transportMessage;

            OnPipelineStartedForSend(message);

            try
            {
                // serialize the message to a byte array and send the message to the target(s):
                byte[] payload = _serializer.SerializeToBytes(message);
                transportMessage = new TransportMessage(payload);
                transportMessage.SetPayloadType(message.GetType().Name);

                foreach (ISubscription subscription in subscriptions)
                {
                    try
                    {
                        transport.Send(subscription.Location, transportMessage);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            OnPipelineCompletedForSend(transportMessage);
        }


        public IDomainEvent InvokeForReceive(ITransportMessage message)
        {
            IDomainEvent domainEvent;

            OnPipelineStartedForReceive(message);

            try
            {
                // deserialize the byte array into a message and send for processing:
                object payload = _serializer.Deserialize(message.Getpayload<byte[]>());
                domainEvent = payload as IDomainEvent;
            }
            catch (Exception e)
            {
                throw e;
            }

            OnPipelineCompletedForReceive(domainEvent);

            return domainEvent;
        }

        private void OnPipelineStartedForReceive(ITransportMessage message)
        {
            ICollection<AbstractPipelineModule> pipelineModules = FindAllModules();
            if (pipelineModules.Count == 0) return;

            foreach (AbstractPipelineModule module in pipelineModules)
            {
                try
                {
                    var args = new PipelineManagerPipelineStartedForReceiptEventArgs(message);
                    module.OnPipelineStartedForReceipt(args);
                }
                catch (Exception e)
                {
                }
            }
        }

        private void OnPipelineCompletedForReceive(IDomainEvent @event)
        {
            ICollection<AbstractPipelineModule> pipelineModules = FindAllModules();
            if (pipelineModules.Count == 0) return;

            foreach (AbstractPipelineModule module in pipelineModules)
            {
                try
                {
                    var args = new PipelineManagerPipelineCompletedForReceiptEventArgs(@event);
                    module.OnPipelineCompletedForReceipt(args);
                }
                catch (Exception e)
                {
                }
            }
        }


        private void OnPipelineCompletedForSend(ITransportMessage message)
        {
            ICollection<AbstractPipelineModule> pipelineModules = FindAllModules();
            if (pipelineModules.Count == 0) return;

            foreach (AbstractPipelineModule module in pipelineModules)
            {
                try
                {
                    var args = new PipelineManagerPipelineCompletedForSendingEventArgs(message);
                    module.OnPipelineCompletedForSend(args);
                }
                catch (Exception e)
                {
                }
            }
        }

        private void OnPipelineStartedForSend(object message)
        {
            ICollection<AbstractPipelineModule> pipelineModules = FindAllModules();
            if (pipelineModules.Count == 0) return;

            foreach (AbstractPipelineModule module in pipelineModules)
            {
                try
                {
                    var args = new PipelineManagerPipelineStartedForSendingEventArgs(message);
                    module.OnPipelineStartedForSend(args);
                }
                catch (Exception e)
                {
                }
            }
        }

        private ICollection<AbstractPipelineModule> FindAllModules()
        {
            ICollection<AbstractPipelineModule> modules = new List<AbstractPipelineModule>();

            try
            {
                modules = _kernel.ResolveAll<AbstractPipelineModule>();
            }
            catch (Exception e)
            {
            }

            return modules;
        }

        private void InvokePipelineStartedForSendEvent(PipelineManagerPipelineStartedForSendingEventArgs e)
        {
            Action<PipelineManagerPipelineStartedForSendingEventArgs> Handler = PipelineStartedForSendEvent;
            if (Handler != null) 
                Handler(e);
        }
    }
}