using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Halifax.Bus.Eventing.Async.Endpoints.Module;
using Halifax.Bus.Eventing.Async.Transport;

namespace Halifax.Bus.Eventing.Async.Endpoints
{
    public class EndpointManager : IEndpointManager, IDisposable
    {
        private static readonly object _endpoint_lock = new object();
        private readonly IKernel _kernel;
        private readonly List<BaseTransport> _registeredEndpoints;

        public EndpointManager(IKernel kernel)
        {
            _kernel = kernel;

            if (_registeredEndpoints == null)
                _registeredEndpoints = new List<BaseTransport>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion

        #region IEndpointManager Members

        public event EventHandler<TransportMessageReceivedEventArgs> EndpointTransportMessageReceivedEvent;
        public event EventHandler<TransportErrorEventArgs> EndpointTransportErrorEvent;

        public bool IsRunning { get; private set; }

        public void Start()
        {
            if (IsRunning) return;

            foreach (BaseTransport endpoint in _registeredEndpoints)
            {
                endpoint.TransportStartReceiveMessageEvent += OnTransportStartReceive;
                endpoint.TransportEndReceiveMessageEvent += OnTransportEndReceive;
                endpoint.TransportMessageReceived += OnTransportMessageReceived;
                endpoint.TransportError += OnTransportError;
                endpoint.Start();
            }

            IsRunning = true;
        }

        public void Stop()
        {
            foreach (BaseTransport endpoint in _registeredEndpoints)
            {
                endpoint.TransportStartReceiveMessageEvent -= OnTransportStartReceive;
                endpoint.TransportEndReceiveMessageEvent -= OnTransportEndReceive;
                endpoint.TransportMessageReceived -= OnTransportMessageReceived;
                endpoint.TransportError -= OnTransportError;
                endpoint.Stop();
            }

            IsRunning = false;
        }

        public void RegisterEndpoint(string location)
        {
            var transport = _kernel.Resolve<ITransport>() as BaseTransport;
            transport.Uri = location;

            if (!_registeredEndpoints.Contains(transport))
                lock (_endpoint_lock)
                    _registeredEndpoints.Add(transport);
        }

        public void UnregisterEndpoint(string location)
        {
            BaseTransport endpoint = (from ep in _registeredEndpoints
                                      where ep.Uri == location
                                      select ep).FirstOrDefault();

            if (endpoint != null)
                lock (_endpoint_lock)
                    _registeredEndpoints.Remove(endpoint);
        }

        #endregion

        private void OnTransportStartReceive(object sender, TransportStartReceivedMessageEventArgs e)
        {
            try
            {
                IEnumerable<IEndpointModule> modules = FindAllEndpointModules(e.Uri);
                if (modules.Count() == 0) return;

                foreach (IEndpointModule module in modules)
                {
                    try
                    {
                        module.OnEndpointStartReceive(e);
                    }
                    catch (Exception exception)
                    {
                    }
                }
            }
            catch (Exception exception)
            {
                OnTransportError(this, new TransportErrorEventArgs(e.Uri, e.Message, exception));
            }
        }

        private void OnTransportEndReceive(object sender, TransportEndReceivedMessageEventArgs e)
        {
            try
            {
                IEnumerable<IEndpointModule> modules = FindAllEndpointModules(e.Uri);
                if (modules.Count() == 0) return;

                foreach (IEndpointModule module in modules)
                {
                    try
                    {
                        module.OnEndpointCompleteReceive(e);
                    }
                    catch (Exception exception)
                    {
                    }
                }
            }
            catch (Exception exception)
            {
                OnTransportError(this, new TransportErrorEventArgs(e.Uri, e.Message, exception));
            }
        }

        private void OnTransportError(object sender, TransportErrorEventArgs e)
        {
            EventHandler<TransportErrorEventArgs> evt = EndpointTransportErrorEvent;

            if (evt != null)
                evt(this, new TransportErrorEventArgs(e.Location, e.Message, e.Exception));
        }

        private void OnTransportMessageReceived(object sender, TransportMessageReceivedEventArgs e)
        {
            EventHandler<TransportMessageReceivedEventArgs> evt = EndpointTransportMessageReceivedEvent;
            if (evt != null)
                evt(this, new TransportMessageReceivedEventArgs(e.Location, e.Message));
        }

        private IEnumerable<IEndpointModule> FindAllEndpointModules(string location)
        {
            IEnumerable<IEndpointModule> retval = new List<IEndpointModule>();

            try
            {
                IEndpointModule[] endpointModules = _kernel.ResolveAll<IEndpointModule>();
                if (endpointModules.Count() == 0) return retval;

                retval = from m in endpointModules
                         where m.Locations.Contains(location)
                         select m;
            }
            catch (Exception e)
            {
            }

            return retval;
        }
    }
}