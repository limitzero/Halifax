using System;
using System.Collections.Generic;
using Halifax.Bus.Eventing.Async.Transport;

namespace Halifax.Bus.Eventing.Async.Endpoints.Module
{
    /// <summary>
    /// Contract for a place in the messaging receipt 
    /// where some intermediary actions can be done
    /// before and after the message is processed 
    /// by the transport
    /// </summary>
    public interface IEndpointModule : IDisposable
    {
        /// <summary>
        /// (Read-Write). Series of message receipt locations where the module will be run
        /// </summary>
        ICollection<string> Locations { get; set; }

        /// <summary>
        /// Interception point to do work before the message is received, but before it is processed.
        /// </summary>
        /// <param name="args"></param>
        void OnEndpointStartReceive(TransportStartReceivedMessageEventArgs args);

        /// <summary>
        /// Interception point to do work after the message is received and processed.
        /// </summary>
        /// <param name="args"></param>
        void OnEndpointCompleteReceive(TransportEndReceivedMessageEventArgs args);
    }
}