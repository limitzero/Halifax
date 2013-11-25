using System;
using Halifax.Configuration;
using Halifax.Tests.Samples.OnlineOrdering.Domain.Services;

namespace Halifax.Tests.Samples.OnlineOrdering
{
	public class OnlineOrderingContainerConfigurator : ICanConfigureContainer
	{
		public void Configure(IContainer container)
		{
			container.Register<IItemsInCurrentCartService, ItemsInCurrentCartService>();
		}
	}
}