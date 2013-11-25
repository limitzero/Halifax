using System;
using Halifax.Commands;

namespace Halifax.Internals.Commands
{
    public class CommndBusCompletedPublishMessageEventArgs : EventArgs
    {
        public CommndBusCompletedPublishMessageEventArgs(Command command)
        {
            Command = command;
        }

        public Command Command { get; set; }
    }
}