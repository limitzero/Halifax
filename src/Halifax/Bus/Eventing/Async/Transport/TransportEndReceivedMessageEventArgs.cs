using System;

namespace Halifax.Bus.Eventing.Async.Transport
{
    public class TransportEndReceivedMessageEventArgs : EventArgs
    {
        public TransportEndReceivedMessageEventArgs(string uri, ITransportMessage message)
        {
            Uri = uri;
            Message = message;
        }

        public string Uri { get; set; }
        public ITransportMessage Message { get; private set; }
    }
}