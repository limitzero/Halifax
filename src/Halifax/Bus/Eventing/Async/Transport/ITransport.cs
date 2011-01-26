using System;

namespace Halifax.Bus.Eventing.Async.Transport
{
    public interface ITransport
    {
        /// <summary>
        /// (Read-Write). Denotes whether or not the transport will receive and 
        /// send messages in a transaction.
        /// </summary>
        bool IsTransactional { get; set; }

        /// <summary>
        /// (Read-Write). Location to retreive messages from.
        /// </summary>
        string Uri { get; set; }

        event EventHandler<TransportStartReceivedMessageEventArgs> TransportStartReceiveMessageEvent;
        event EventHandler<TransportEndReceivedMessageEventArgs> TransportEndReceiveMessageEvent;

        event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;
        event EventHandler<TransportMessageDeliveredEventArgs> TransportMessageDelivered;
        event EventHandler<TransportErrorEventArgs> TransportError;

        /// <summary>
        /// This will send a series of messages to the indicated location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="messages"></param>
        void Send(string location, params ITransportMessage[] messages);

        /// <summary>
        /// This will receive a message from a location as specified by the 
        /// setting of <seealso cref="Uri"/>.
        /// </summary>
        /// <returns></returns>
        ITransportMessage Receive();
    }
}