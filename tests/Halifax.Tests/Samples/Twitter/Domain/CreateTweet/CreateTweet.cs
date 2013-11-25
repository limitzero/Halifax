using Halifax.Commands;

namespace Halifax.Tests.Samples.Twitter.Domain.CreateTweet
{
	public class CreateTweetCommand : Command
	{
		public string User { get; set; }
		public string Message { get; set; }

		public CreateTweetCommand(string user, string message)
		{
			User = user;
			Message = message;
		}
	}
}