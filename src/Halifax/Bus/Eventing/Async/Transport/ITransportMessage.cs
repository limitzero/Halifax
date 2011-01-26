namespace Halifax.Bus.Eventing.Async.Transport
{
    public interface ITransportMessage
    {
        object Payload { get; }
        string PayloadType { get; }
        TPayload Getpayload<TPayload>();
        void SetPayloadType(string type);
    }
}