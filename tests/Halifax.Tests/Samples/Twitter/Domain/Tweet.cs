using System;
using Halifax.Domain;
using Halifax.Read;
using Halifax.Tests.Samples.Twitter.Domain.CreateTweet;

namespace Halifax.Tests.Samples.Twitter.Domain
{
	// aggregate root used for behavioral model and persistance model...no event sourcing!!! 
	// much like this:  http://www.udidahan.com/2008/02/29/how-to-create-fully-encapsulated-domain-models/
	public class Tweet : AggregateRoot, IReadModel
	{
		public virtual string User { get; set; }
		public virtual string Message { get; set; }
		public virtual DateTime At { get; set; }

		public virtual void Create(string user, string message)
		{
			if (true /* do your logic and if successful, set the state and apply the event   */)
			{
				// create your event:
				var @event = new TweetCreated
				             	{
				             		Message = message,
				             		User = user
				             	};

				// set your state as normal (i.e. properties):
				this.User = @event.User;
				this.Message = @event.Message;
				this.At = @event.At;

				// send the event (persistance happens here):
				Apply(@event);
			}
		}
	}
}