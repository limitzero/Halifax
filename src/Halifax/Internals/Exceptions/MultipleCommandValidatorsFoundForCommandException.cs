using Halifax.Commands;

namespace Halifax.Internals.Exceptions
{
	public class MultipleCommandValidatorsFoundForCommandException : HalifaxException
	{
		private const string _message =
			"There were more than one command validator registered for the command '{0}'. " +
			"Please make sure that only one command validator can process the command '{1}.'";

		public MultipleCommandValidatorsFoundForCommandException(Command command)
			: base(string.Format(_message, command.GetType().Name, command.GetType().Name))
		{
		}
	}
}