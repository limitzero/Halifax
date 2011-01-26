namespace Halifax.Bus.Eventing.Async.Transport
{
    public class NullTransportMessage : TransportMessage
    {
        public NullTransportMessage()
            : base(null)
        {
        }
    }
}