using System;

namespace Halifax.Bus.Eventing.Async.RuntimeServices
{
    public class BackGroundServiceErrorEventArgs : EventArgs
    {
        public BackGroundServiceErrorEventArgs()
        {
        }

        public BackGroundServiceErrorEventArgs(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        public string Message { get; private set; }
        public Exception Exception { get; private set; }
    }
}