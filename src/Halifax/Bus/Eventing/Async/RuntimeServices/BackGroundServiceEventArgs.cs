using System;

namespace Halifax.Bus.Eventing.Async.RuntimeServices
{
    public class BackGroundServiceEventArgs : EventArgs
    {
        public BackGroundServiceEventArgs()
        {
        }

        public BackGroundServiceEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}