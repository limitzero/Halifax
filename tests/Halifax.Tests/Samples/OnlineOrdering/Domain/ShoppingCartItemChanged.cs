using System;
using Halifax.Events;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain
{
	public class ShoppingCartItemChanged : Event
	{
		public Guid ItemId { get; set; }
		public string UserName { get; set; }
	}
}