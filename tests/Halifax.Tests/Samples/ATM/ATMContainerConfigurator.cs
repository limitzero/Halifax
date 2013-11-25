using Halifax.Configuration;
using Halifax.Tests.Samples.ATM.Services;

namespace Halifax.Tests.Samples.ATM
{
	public class ATMContainerConfigurator : ICanConfigureContainer
	{
		public void Configure(IContainer container)
		{
			container.Register<IOverdraftInspectionService, OverdraftInspectionService>();
		}
	}
}