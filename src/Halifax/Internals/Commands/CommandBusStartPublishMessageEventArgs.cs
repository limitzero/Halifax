using System;
using Halifax.Commands;
using Halifax.Events;

namespace Halifax.Internals.Commands
{
    public class CommandBusStartPublishMessageEventArgs : EventArgs
    {
        public CommandBusStartPublishMessageEventArgs(Command command)
        {
            Command = command;
        }

        public Command Command { get; set; }
        public Event DomainEvent { get; set; }
    }
}