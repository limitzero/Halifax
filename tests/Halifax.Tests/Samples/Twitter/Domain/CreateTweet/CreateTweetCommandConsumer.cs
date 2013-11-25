using System;
using Halifax.Commands;
using Halifax.Domain;

namespace Halifax.Tests.Samples.Twitter.Domain.CreateTweet
{
	public class CreateTweetCommandConsumer :
			CommandConsumer.For<CreateTweetCommand>
	{
		private readonly IAggregateRootRepository repository;

		public CreateTweetCommandConsumer(IAggregateRootRepository repository)
		{
			this.repository = repository;
		}

		public override AggregateRoot Execute(CreateTweetCommand command)
		{
			var tweet = this.repository.Get<Tweet>(command.Id);
			tweet.Create(command.User, command.Message);
			return tweet;
		}
	}
}