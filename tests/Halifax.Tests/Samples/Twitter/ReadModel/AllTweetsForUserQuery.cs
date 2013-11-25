using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Read;
using Halifax.Tests.Samples.Twitter.Domain;

namespace Halifax.Tests.Samples.Twitter.ReadModel
{
	public class AllTweetsForUserQuery : Query<Tweet, IEnumerable<Tweet>>
	{
		private readonly string username;

		public AllTweetsForUserQuery(string username)
		{
			this.username = username;
		}

		public override void Execute(IQueryable<Tweet> queryable)
		{
			this.Result = queryable.Where(model => model.User.Equals(this.username)).ToList();
		}
	}
}