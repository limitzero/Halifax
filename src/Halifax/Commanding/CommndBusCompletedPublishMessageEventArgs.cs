using System;

namespace Halifax.Commanding
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