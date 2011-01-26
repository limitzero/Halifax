using System;

namespace Halifax.Testing
{
    public class NoExceptionWasCaughtButOneWasExpectedException : ApplicationException
    {
        public NoExceptionWasCaughtButOneWasExpectedException()
            : base("No exception was caught but one was expected.")
        {
        }
    }
}