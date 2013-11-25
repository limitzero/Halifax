using System;
using Halifax.Read;

namespace Halifax.Tests.Samples.OnlineOrdering.ReadModel
{
	public class ShoppingCart : IReadModel
	{
		public Guid Id { get; set; }
		public string UserName { get; set; }
		public DateTime ValidUntil { get; set; }
	}
}