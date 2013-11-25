using System;
using Halifax.Commands;

namespace Halifax.Internals.Commands
{
    /// <summary>
    /// Contract for message bus structure that will process command messages.
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// Event that is triggered when the comand bus is starting to publish a message to the external command handler.
        /// </summary>
        event EventHandler<CommandBusStartPublishMessageEventArgs> CommandBusStartMessagePublishEvent;

        /// <summary>
        /// Event that is triggered when the command bus has completed publishing a message to the external command handler.
        /// </summary>
        event EventHandler<CommndBusCompletedPublishMessageEventArgs> CommandBusCompletedMessagePublishEvent;

        /// <summary>
        /// This will send the current commmand message to the appropriate 
        /// command handler and return a result to the caller. 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        CommandResult Send<TCommand>(TCommand command) where TCommand : Command;
    }
}