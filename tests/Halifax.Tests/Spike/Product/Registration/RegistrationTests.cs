using Halifax.Configuration;
using Halifax.Configuration.Impl.Containers.Impl;
using Machine.Specifications;

namespace Halifax.Tests.Spike.Product.Registration
{
	[Subject("component auto-registration")]
	public class when_the_configuration_component_is_asked_to_configure_the_dependencies
	{
		private static Configuration.Impl.Configuration configuration;
		private static TestConfigurator configurator;

		Establish context = () =>
		                    	{
									// created by infrastructure:
									configuration = new Configuration.Impl.Configuration();
									configurator = new TestConfigurator();
		                    	};

		Because of = () => configurator.Configure(configuration);

		It will_return_the_current_inversion_of_control_container = () =>
		{
			configuration.CurrentContainer().ShouldBeOfType<CastleWindsorContainer>();
		};
	}

	// this will be supplied to the infrastructure for configuring our components:
	public class TestConfigurator : IConfigurator
	{
		public void Configure(IConfiguration configuration)
		{
			configuration
			    .Container (c => c.UsingCastleWindsor())
				.Eventing( ev => ev.Synchronous())
				.EventStore(es => es.UsingInMemoryStorage())
				.Serialization(s =>s.UsingSharpSerializer())
				.ReadModel (rm => rm.UsingInMemoryRepository())
				.Configure(this.GetType().Assembly);
		}
	}
}