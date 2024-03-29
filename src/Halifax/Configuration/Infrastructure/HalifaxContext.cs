using System;
using Castle.Windsor;
using Halifax.Bus.Commanding;
using Halifax.Bus.Eventing;
using Halifax.Commanding;

namespace Halifax.Configuration.Infrastructure
{
    /// <summary>
    /// Local communication context to abstract the internals away 
    /// from the client code for passing commands onto the consumers
    /// for invoking actions on the aggregates.
    /// </summary>
    public class HalifaxContext : IDisposable
    {
        private const string _default_configuration_file = @"halifax.config.xml";
        private readonly string _configurationFile;
        private IStartableCommandBus _commandBus;
        private IWindsorContainer _container;
        private IStartableEventBus _eventBus;

        /// <summary>
        /// This will initiate the core context
        /// from the application configuration 
        /// file that includes the facility and 
        /// components designed for the Castle
        /// container. It will look for a file 
        /// by the name of 'halifax.config.xml' 
        /// in the executable directory of the 
        /// current application to bootstrap 
        /// the core context.
        /// </summary>
        public HalifaxContext()
            : this(_default_configuration_file)
        {
        }

        /// <summary>
        /// This will intiate the core context with 
        /// a file that has the configuraton for the 
        /// infrastructure designed for the Castle
        /// container.
        /// </summary>
        /// <param name="configurationFile"></param>
        public HalifaxContext(string configurationFile)
        {
            _configurationFile = configurationFile;
            BootstrapFrom(configurationFile);
        }

        public void Dispose()
        {
            if (_commandBus.IsRunning)
                _commandBus.Stop();

            if (_eventBus.IsRunning)
                _eventBus.Stop();

            if (_container != null)
                _container.Dispose();
        }

        /// <summary>
        /// This will read the default configuration file that configures 
        /// the command consumers, event consumers, events 
        /// and messages for interacting with the domain.
        /// </summary>
        public void Bootstrap()
        {
            if (!string.IsNullOrEmpty(_configurationFile))
                BootstrapFrom(_configurationFile);
            else
            {
                BootstrapFrom(string.Empty);
            }
        }
        
        /// <summary>
        /// This will take a configuration file that configures 
        /// the command consumers, event consumers, events 
        /// and messages for interacting with the domain.
        /// </summary>
        /// <param name="configurationFile"></param>
        public void BootstrapFrom(string configurationFile)
        {
            if (_container != null) return; // already configured

            if (!string.IsNullOrEmpty(configurationFile))
                _container = new WindsorContainer(configurationFile);
            else
            {
                _container = new WindsorContainer();
            }

            _container.AddFacility(HalifaxFacility.FACILITY_ID, new HalifaxFacility());
            _commandBus = _container.Resolve<IStartableCommandBus>();
            _eventBus = _container.Resolve<IStartableEventBus>();

            _commandBus.Start();
            _eventBus.Start();
        }

        /// <summary>
        /// This will send the command to the domain via the command 
        /// consumers and forward any triggered events to their 
        /// corresponding event consumers.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public void Send(Command command)
        {
            if (_container == null)
                throw new Exception(
                    "Before sending the command, make sure to invoke the BootStrap() or BootStrapFrom(...) methods to initialize the communcation context.");

            if (_commandBus.IsRunning  == false)
                throw new Exception("The current environment has not been started for sending commands.");

            if (_eventBus.IsRunning == false)
                throw new Exception("The current environment has not been started for publishing events.");

            _commandBus.Send(command);
        }

		/// <summary>
		/// This will send the command to the domain via the command 
		/// consumers and forward any triggered events to their 
		/// corresponding event consumers. This implements the BeginXXX and 
		/// EndXXX pattern for async events.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public IAsyncResult SendAsync(Command command, AsyncCallback callback = null)
		{
			Action sendCommand = () => this.Send(command);
			IAsyncResult asyncResult = sendCommand.BeginInvoke(callback, null);
			return asyncResult;
		}

    	/// <summary>
		/// This will find a component in the underlying container.
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <returns></returns>
		public TComponent Resolve<TComponent>()
		{
			return _container.Resolve<TComponent>();
		}

		private void SendCommandAsync(object state)
		{
			Command command = state as Command;

			if(command != null)
			{
				this.Send(command);
			}
		}
    }
}