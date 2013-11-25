using System;
using System.Threading;
using Halifax.Commands;
using Halifax.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Halifax.SignalR.Tests.given
{
	public interface IHalifaxSignalRHost : IDisposable
	{
		/// <summary>
		/// Starts the host and keeps the host alive for a given amount 
		/// of time. 
		/// </summary>
		/// <param name="timeToLiveInSeconds">Time in seconds to keep the host alive. Zero (0) keeps the host open indefintely until Dispose is called</param>
		void Start(int timeToLiveInSeconds = 0);
	}

	public class HalifaxSignalRHost : IHalifaxSignalRHost
	{
		private readonly Uri endpoint;
		private bool started = false;
		private bool disposed = false;
		private IDisposable SignalR;
		private Thread worker; 

		public HalifaxSignalRHost(Uri endpoint)
		{
			this.endpoint = endpoint;
		}

		public void Dispose()
		{
			this.Disposing(true);
			GC.SuppressFinalize(this);
		}

		public void Start(int timeToLiveInSeconds)
		{
			GuardAgainst(() => this.disposed == true, "Cannot access a disposed instance of the Halifax SignalR host.");
			if(GuardAgainst(()=> this.started == true, string.Empty)) return;

			if(timeToLiveInSeconds > 0)
			{
				ManualResetEvent wait = new ManualResetEvent(false);
				this.InvokeHost();
				wait.WaitOne(TimeSpan.FromSeconds(timeToLiveInSeconds));
				wait.Set();
			}

			this.started = true;
		}

		private void InvokeHost()
		{
			//this.SignalR = WebApp.Start<HalifaxSignalRStartup>(endpoint.ToString());
		}

		private void Disposing(bool disposing)
		{
			if(disposing == true)
			{
				if(this.SignalR != null)
				{
					this.SignalR.Dispose();
					this.SignalR = null;
				}
			}
			this.disposed = true;
		}

		private static bool GuardAgainst(Func<bool> guardAgainst, string message)
		{
			bool result = guardAgainst();
			
			if(result == false && string.IsNullOrEmpty(message) == false)
			{
				throw new InvalidOperationException(message);
			}

			return result;
		}
	}

	[HubName("halifax")]
	public class HalifaxHub : Hub
	{
		private readonly IConfiguration configuration;

		public HalifaxHub(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public CommandResult ProcessCommand(Command command)
		{
			CommandResult result = null;
			return result;
		}
	}
}