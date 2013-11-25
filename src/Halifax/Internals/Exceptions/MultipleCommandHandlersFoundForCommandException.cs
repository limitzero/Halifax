using Halifax.Commands;

namespace Halifax.Internals.Exceptions
{
    public class MultipleCommandHandlersFoundForCommandException : HalifaxException
    {
        private const string _message =
            "There were more than one command handlers registered for the command '{0}'. " +
            "Please make sure that only one command handler can process the command '{1}.'";

        public MultipleCommandHandlersFoundForCommandException(Command command)
            : base(string.Format(_message, command.GetType().Name, command.GetType().Name))
        {
        }
    }
}