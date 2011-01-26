using System;

namespace Halifax.Bus.Eventing.Async.Transport
{
    public class TransportMessageReceivedEventArgs : EventArgs
    {
        public TransportMessageReceivedEventArgs(string location, ITransportMessage message)
        {
            Location = location;
            Message = message;
        }

        public string Location { get; set; }
        public ITransportMessage Message { get; set; }
    }
}