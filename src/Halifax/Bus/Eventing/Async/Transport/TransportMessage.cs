namespace Halifax.Bus.Eventing.Async.Transport
{
    public class TransportMessage : ITransportMessage
    {
        public TransportMessage(object payload)
        {
            Payload = payload;

            if (!(payload is byte[]))
                PayloadType = payload.GetType().Name;
        }

        #region ITransportMessage Members

        public string PayloadType { get; private set; }

        public object Payload { get; private set; }

        public void SetPayloadType(string type)
        {
            PayloadType = type;
        }

        public TPayload Getpayload<TPayload>()
        {
            return (TPayload) Payload;
        }

        #endregion
    }
}