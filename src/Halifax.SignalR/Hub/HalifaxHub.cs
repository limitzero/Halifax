using Halifax.Commands;
using Halifax.Configuration;
using Halifax.Configuration.Impl.Serialization;
using Halifax.Internals.Commands;
using Microsoft.AspNet.SignalR.Hubs;

namespace Halifax.SignalR.Hub
{
	[HubName("halifax")]
	public class HalifaxHub : Microsoft.AspNet.SignalR.Hub
	{
		private readonly IConfiguration configuration;

		public HalifaxHub(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		/// <summary>
		/// This will send the command to the infrastructure 
		/// by name and JSON payload for re-consituting the 
		/// command object on the .NET side for processing.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="message"></param>
		public void SendCommand(string name, string message)
		{
			var serialization = configuration.CurrentContainer().Resolve<ISerializationProvider>();
			var command = serialization.Deserialize(name, message) as Command;

			if(command == null) return;

			var result = configuration.CurrentContainer().Resolve<ICommandBus>().Send(command);
		}
	}
}