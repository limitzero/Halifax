using System;
using Halifax.Eventing;

namespace Halifax.Commanding
{
    public class CommandBusStartPublishMessageEventArgs : EventArgs
    {
        public CommandBusStartPublishMessageEventArgs(Command command)
        {
            Command = command;
        }

        public Command Command { get; set; }
        public IDomainEvent DomainEvent { get; set; }
    }
}