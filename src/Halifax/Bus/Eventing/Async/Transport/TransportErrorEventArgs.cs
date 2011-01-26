using System;

namespace Halifax.Bus.Eventing.Async.Transport
{
    public class TransportErrorEventArgs : EventArgs
    {
        public TransportErrorEventArgs(string location, ITransportMessage message, Exception exception)
        {
            Location = location;
            Message = message;
            Exception = exception;
        }

        public string Location { get; private set; }
        public ITransportMessage Message { get; private set; }
        public Exception Exception { get; private set; }
    }
}