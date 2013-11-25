using System.Linq;
using Halifax.Tests.Samples.Twitter.Domain.CreateTweet;
using Halifax.Tests.Samples.Twitter.ReadModel;
using Machine.Specifications;

namespace Halifax.Tests.Samples.Twitter.Tests
{
	// this will use the aggregate root (behavioral or write-model) as the read model, 
	// essentially doing DDD without event sourcing (CQRS+DM) using the central context 
	// for brokering the command/query communication:
	[Subject("tweets")]
	public class when_a_user_creates_a_tweet
	{
		private static string user;
		private static string message;

		Establish context = () =>
			                    {
									user = "jdoe";
									message = "hello";

			                    	HalifaxContext.ConfigurationFactory
										.ConfigureWith()
												.Container(c => c.UsingCastleWindsor())
												.Eventing(ev => ev.Synchronous())
												.EventStore(es => es.UsingInMemoryStorage())
												.Serialization(s => s.UsingSharpSerializer())
												.ReadModel(rm => rm.UsingInMemoryRepository())
												.Configure(typeof(when_changing_the_price_of_a_product).Assembly);
			                    };

		Because of = () => Halifax.HalifaxContext.ConfigurationFactory.SendCommand(
			new CreateTweetCommand(user, message));

		It will_display_all_tweets_created_from_the_user = () =>
			                                                    {
			                                                    	int number_of_tweets = 
																		Halifax.HalifaxContext.ConfigurationFactory.QueryBy(
			                                                    		new AllTweetsForUserQuery(user)).Count();
																	number_of_tweets.ShouldEqual(1);
			                                                    };
		
	}

	/*
	public class XUnitSpecificationRunner : SpecificationRunner
	{
		[Fact]
		public void RunAll()
		{
			this.Run();
		}
	}
	 */
}