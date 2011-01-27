using System;
using Halifax.Bus.Eventing.Async.Transport;
using Halifax.Internals;

namespace Halifax.Bus.Eventing.Async.Endpoints
{
    public interface IEndpointManager : IStartable
    {
        event EventHandler<TransportMessageReceivedEventArgs> EndpointTransportMessageReceivedEvent;
        event EventHandler<TransportErrorEventArgs> EndpointTransportErrorEvent;

        void RegisterEndpoint(string location);
        void UnregisterEndpoint(string location);
    }
}