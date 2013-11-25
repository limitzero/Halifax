using Microsoft.AspNet.SignalR;
using Owin;

namespace Halifax.SignalR.Tests.given
{
	public class HalifaxSignalRStartup
	{
		public static IAppBuilder Builder = null;

		public void Configure(IAppBuilder appBuilder)
		{
			var hubConfiguration = new HubConfiguration
			                       	{
			                       		EnableCrossDomain = true,
			                       		EnableDetailedErrors = true
			                       	};

			//appBuilder.MapHubs(hubConfiguration);
		}
	}
}