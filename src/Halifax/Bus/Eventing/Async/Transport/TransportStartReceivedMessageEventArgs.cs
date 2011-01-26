using System;

namespace Halifax.Bus.Eventing.Async.Transport
{
    public class TransportStartReceivedMessageEventArgs : EventArgs
    {
        public TransportStartReceivedMessageEventArgs(string uri, ITransportMessage message)
        {
            Uri = uri;
            Message = message;
        }

        public string Uri { get; set; }
        public ITransportMessage Message { get; private set; }
    }
}