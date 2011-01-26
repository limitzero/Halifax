using System;
using System.Transactions;
using Halifax.Bus.Eventing.Async.RuntimeServices;

namespace Halifax.Bus.Eventing.Async.Transport
{
    public abstract class BaseTransport : BaseBackgroundService, ITransport
    {
        #region ITransport Members

        public event EventHandler<TransportStartReceivedMessageEventArgs> TransportStartReceiveMessageEvent;
        public event EventHandler<TransportEndReceivedMessageEventArgs> TransportEndReceiveMessageEvent;
        public event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;
        public event EventHandler<TransportMessageDeliveredEventArgs> TransportMessageDelivered;
        public event EventHandler<TransportErrorEventArgs> TransportError;

        public virtual bool IsTransactional { get; set; }
        public virtual string Uri { get; set; }

        public abstract void Send(string location, params ITransportMessage[] messages);

        public abstract ITransportMessage Receive();

        #endregion

        public override void PerformAction()
        {
            DoReceive();
        }

        private void DoReceive()
        {
            ITransportMessage message = null;

            OnTransportStartReceive();

            using (var txn = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    message = Receive();
                    if (!(message is NullTransportMessage))
                        OnTransportMesssageReceived(message);

                    txn.Complete();
                }
                catch (Exception e)
                {
                    if (!OnTransportError(e, message))
                        throw;
                }
            }

            OnTransportEndReceive(message);
        }

        private void OnTransportStartReceive()
        {
            EventHandler<TransportStartReceivedMessageEventArgs> evt = TransportStartReceiveMessageEvent;
            if (evt != null)
                evt(this, new TransportStartReceivedMessageEventArgs(Uri, new NullTransportMessage()));
        }

        private void OnTransportEndReceive(ITransportMessage message)
        {
            EventHandler<TransportEndReceivedMessageEventArgs> evt = TransportEndReceiveMessageEvent;
            if (evt != null)
                evt(this, new TransportEndReceivedMessageEventArgs(Uri, message));
        }

        private void OnTransportMesssageReceived(ITransportMessage message)
        {
            EventHandler<TransportMessageReceivedEventArgs> evt = TransportMessageReceived;
            if (evt != null)
                evt(this, new TransportMessageReceivedEventArgs(Uri, message));
        }

        private bool OnTransportError(Exception exception, ITransportMessage message)
        {
            EventHandler<TransportErrorEventArgs> evt = TransportError;
            bool isHandlerAttached = (evt != null);

            if (evt != null)
                evt(this, new TransportErrorEventArgs(Uri, message, exception));

            return isHandlerAttached;
        }
    }
}