using System;
using Halifax.Commands;

namespace Halifax.Internals.Exceptions
{
    public class UnRegisteredCommandHandlerForCommandException : HalifaxException
    {
        private const string _message =
            "There was not an command handler registered for the command '{0}'. " +
            "Make sure to create a class that inherits from '{1}<{2}>'  to handle the associated command.";

        public UnRegisteredCommandHandlerForCommandException(Type command)
            : base(string.Format(_message, command.Name, typeof (CommandConsumer.For<>).Name, command.Name))
        {
        }
    }
}