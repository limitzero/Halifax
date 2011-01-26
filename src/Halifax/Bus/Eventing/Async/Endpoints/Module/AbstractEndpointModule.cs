using System.Collections.Generic;
using Halifax.Bus.Eventing.Async.Transport;

namespace Halifax.Bus.Eventing.Async.Endpoints.Module
{
    public abstract class AbstractEndpointModule : IEndpointModule
    {
        #region IEndpointModule Members

        public ICollection<string> Locations { get; set; }

        public virtual void OnEndpointStartReceive(TransportStartReceivedMessageEventArgs args)
        {
        }

        public virtual void OnEndpointCompleteReceive(TransportEndReceivedMessageEventArgs args)
        {
        }

        public void Dispose()
        {
            OnEndpointModuleDisposing();
        }

        #endregion

        public void AddLocations(params string[] locations)
        {
            Locations = locations;
        }

        public virtual void OnEndpointModuleDisposing()
        {
        }
    }
}