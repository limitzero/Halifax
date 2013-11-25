using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Commands;
using Halifax.Configuration;
using Halifax.Configuration.Impl.Eventing;
using Halifax.Internals.Commands.Module;
using Halifax.Internals.Dispatchers;

namespace Halifax.Internals.Commands
{
    public class InProcessCommandBus : ICommandBus
    {
        private readonly ICommandMessageDispatcher _dispatcher;
    	private readonly IEventBus event_bus;
    	private readonly IContainer _container;

        public InProcessCommandBus(
            IContainer container,
            ICommandMessageDispatcher dispatcher, 
			IEventBus eventBus)
        {
            _container = container;
            _dispatcher = dispatcher;
        	event_bus = eventBus;
        }

        #region IStartableCommandBus Members

        /// <summary>
        /// Event that is triggered when the comand bus is starting to publish a message to the external command handler.
        /// </summary>
        public event EventHandler<CommandBusStartPublishMessageEventArgs> CommandBusStartMessagePublishEvent;

        /// <summary>
        /// Event that is triggered when the command bus has completed publishing a message to the external command handler.
        /// </summary>
        public event EventHandler<CommndBusCompletedPublishMessageEventArgs> CommandBusCompletedMessagePublishEvent;


        public bool IsRunning { get; private set; }


        public CommandResult Send<TCommand>(TCommand command) where TCommand : Command
        {
			CommandResult result = new CommandResult();

            OnStartSend(command);

            _dispatcher.Dispatch(command);

            OnCompleteSend(command);

        	return result;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            if (IsRunning) return;

            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        #endregion

        private void OnCompleteSend(Command command)
        {
            ICollection<AbstractCommandBusModule> modules = FindAllModules();
            if (modules.Count == 0) return;

            foreach (AbstractCommandBusModule module in modules)
            {
                try
                {
                    var ev = new CommndBusCompletedPublishMessageEventArgs(command);
                    module.OnCommandBusCompletedMessagePublishing(ev);
                }
                catch
                {
                	throw;
                }
            }
        }

        private void OnStartSend(Command command)
        {
            ICollection<AbstractCommandBusModule> modules = FindAllModules();
            if (modules.Count == 0) return;

            foreach (AbstractCommandBusModule module in modules)
            {
                try
                {
                    var ev = new CommandBusStartPublishMessageEventArgs(command);
                    module.OnCommandBusStartMessagePublishing(ev);
                }
                catch
                {
                	throw;
                }
            }
        }

        private ICollection<AbstractCommandBusModule> FindAllModules()
        {
            var retval = new List<AbstractCommandBusModule>();

            try
            {
                IEnumerable<AbstractCommandBusModule> modules = _container.ResolveAll<AbstractCommandBusModule>();

                if (modules.Count() == 0) return retval;

                retval = new List<AbstractCommandBusModule>(modules);
            }
            catch
            {
            	throw;
            }

            return retval;
        }
    }
}