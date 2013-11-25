using Halifax.Commands;

namespace Halifax.Internals.Exceptions
{
    public class NoEventsWerePublishedButSomeWereExpectedException
        : HalifaxException
    {
        public const string _message = "No events were published for the last command of '{0}' but some were expected";

        public NoEventsWerePublishedButSomeWereExpectedException()
        {
        }

        public NoEventsWerePublishedButSomeWereExpectedException(Command command)
            : base(string.Format(_message, command.GetType().Name))
        {
        }
    }
}