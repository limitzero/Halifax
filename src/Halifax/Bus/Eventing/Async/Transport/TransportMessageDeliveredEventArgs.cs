using System;

namespace Halifax.Bus.Eventing.Async.Transport
{
    public class TransportMessageDeliveredEventArgs : EventArgs
    {
        public TransportMessageDeliveredEventArgs(string location, ITransportMessage message)
        {
            Location = location;
            Message = message;
        }

        public string Location { get; private set; }
        public ITransportMessage Message { get; private set; }
    }
}