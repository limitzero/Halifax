using System;
using Halifax.Events;

namespace Halifax.Tests.Samples.Twitter.Domain.CreateTweet
{
	[Serializable]
	public class TweetCreated : Event
	{
		public TweetCreated()
		{
		}

		public TweetCreated(Guid id)
		{
			this.EventSourceId = id;
		}

		public string Message { get; set; }
		public string User { get; set; }
	}
}