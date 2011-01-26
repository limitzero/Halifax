using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Halifax.Commanding;
using Halifax.Commanding.Module;

namespace Halifax.Bus.Commanding
{
    public class InProcessCommandBus : IStartableCommandBus
    {
        private readonly ICommandMessageDispatcher _dispatcher;
        private readonly IKernel _kernel;

        public InProcessCommandBus(
            IKernel kernel,
            ICommandMessageDispatcher dispatcher)
        {
            _kernel = kernel;
            _dispatcher = dispatcher;
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


        public void Send<TCommand>(TCommand command) where TCommand : Command
        {
            OnStartSend(command);

            _dispatcher.Dispatch(command);

            OnCompleteSend(command);
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

        public event Action<Command> OnBusStartMessagePublish;
        public event Action<Command> OnBusCompletedMessagePublish;

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
                catch (Exception e)
                {
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
                catch (Exception e)
                {
                }
            }
        }

        private ICollection<AbstractCommandBusModule> FindAllModules()
        {
            var retval = new List<AbstractCommandBusModule>();

            try
            {
                AbstractCommandBusModule[] modules = _kernel.ResolveAll<AbstractCommandBusModule>();
                if (modules.Length == 0) return retval;
                retval = new List<AbstractCommandBusModule>(modules);
            }
            catch (Exception e)
            {
            }

            return retval;
        }
    }
}