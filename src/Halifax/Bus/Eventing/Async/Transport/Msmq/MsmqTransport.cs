using System;
using System.IO;
using System.Messaging;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Transactions;

namespace Halifax.Bus.Eventing.Async.Transport.Msmq
{
    public class MsmqTransport : BaseTransport
    {
        private MessageQueue _queue;
        private string _uri = string.Empty;

        public MsmqTransport()
        {
            IsTransactional = true;
        }

        public override string Uri
        {
            get { return _uri; }

            set
            {
                _uri = value;
                SetLocalQueue(_uri);
            }
        }

        public override void Send(string location, params ITransportMessage[] messages)
        {
            string path = RetreivePath(location);
            CreateTransactonalQueue(path);

            using (var txn = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    foreach (ITransportMessage message in messages)
                    {
                        var toSend = new Message();

                        using (var queue = new MessageQueue(path, QueueAccessMode.Send))
                        using (var stream = new MemoryStream(message.Getpayload<byte[]>()))
                        {
                            try
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                toSend.BodyStream = stream;
                                toSend.Recoverable = true;
                                toSend.Label = message.PayloadType;

                                queue.Send(toSend, GetTransactionTypeForSend(IsTransactional));
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }

                    txn.Complete();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public override ITransportMessage Receive()
        {
            ITransportMessage message;

            if (IsTransactional)
            {
                using (var txn = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        message = RetreiveMessage();
                        txn.Complete();
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }
            }
            else
            {
                message = RetreiveMessage();
            }

            return message;
        }

        private string RetreivePath(string uri)
        {
            string result = @"FormatName:DIRECT=OS:{0}\Private$\{1}";
            string[] parts = uri.Split(new[] {'@'}); // servername@queuename
            string path = string.Empty;

            if (parts[0].Trim().ToLower() == "localhost" || parts[0].Trim() == ".")
            {
                parts[0] = Environment.MachineName;
                path = string.Format(".\\Private$\\{0}", parts[1].Trim());
                return path;
            }

            IPAddress machineIP;
            if (IPAddress.TryParse(parts[0].Trim(), out machineIP))
                path = string.Format(result, machineIP, parts[1].Trim());
            else
            {
                path = string.Format(result, parts[0].Trim(), parts[1].Trim());
            }

            return path;
        }

        private void SetLocalQueue(string uri)
        {
            string path = RetreivePath(uri);

            try
            {
                CreateTransactonalQueue(path);
            }
            catch
            {
            }

            _queue = new MessageQueue(path);

            var mpf = new MessagePropertyFilter();

            try
            {
                mpf.SetAll();
            }
            catch
            {
            }

            _queue.MessageReadPropertyFilter = mpf;
        }

        private ITransportMessage RetreiveMessage()
        {
            ITransportMessage message = new NullTransportMessage();

            if (TryPeek())
            {
                try
                {
                    Message receivedMessage = _queue.Receive(TimeSpan.FromSeconds(2),
                                                             GetTransactionTypeForSend(IsTransactional));
                    if (receivedMessage != null)
                        message = CreateMessageForMSMQMessage(receivedMessage);
                }
                catch (MessageQueueException mex)
                {
                    // nothing to do here...wait for the next message.
                }
                catch (Exception exception)
                {
                }
            }

            return message;
        }

        private MessageQueueTransactionType GetTransactionTypeForReceive(bool isTransactional)
        {
            if (isTransactional)
                return MessageQueueTransactionType.Automatic;
            return MessageQueueTransactionType.None;
        }

        private MessageQueueTransactionType GetTransactionTypeForSend(bool isTransactional)
        {
            if (isTransactional)
                return MessageQueueTransactionType.Automatic;

            return MessageQueueTransactionType.Single;
        }

        private void CreateTransactonalQueue(string path)
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            if (!MessageQueue.Exists(path))
            {
                MessageQueue.Create(path, true);
            }
            else
            {
                var queue = new MessageQueue(path);
                if (!queue.Transactional)
                    throw new ArgumentException("The queue [" + path + "] must be transactional.");
            }
        }

        private bool TryPeek()
        {
            bool needToHandle = false;

            try
            {
                _queue.Peek(TimeSpan.FromSeconds(1));
                needToHandle = true;
            }
            catch (MessageQueueException mex)
            {
            }
            catch (Exception exception)
            {
            }

            return needToHandle;
        }

        private ITransportMessage CreateMessageForMSMQMessage(Message queueMessage)
        {
            ITransportMessage message = new NullTransportMessage();

            string contents = string.Empty;
            byte[] payload = {};

            try
            {
                using (TextReader reader = new StreamReader(queueMessage.BodyStream))
                {
                    contents = reader.ReadToEnd();
                    payload = Encoding.ASCII.GetBytes(contents);
                }

                message = new TransportMessage(payload);
            }
            catch (Exception exception)
            {
                // just return the string instance of the message:
                message = new TransportMessage(payload);
            }

            return message;
        }
    }
}