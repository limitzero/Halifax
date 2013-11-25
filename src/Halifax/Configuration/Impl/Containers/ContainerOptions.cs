using Halifax.Configuration.Impl.Containers.Impl;

namespace Halifax.Configuration.Impl.Containers
{
	public class ContainerOptions
	{
		private readonly IConfiguration configuration;

		public ContainerOptions(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public ContainerOptions UsingCastleWindsor()
		{
			var container = new CastleWindsorContainer();
			this.configuration.BindContainer(container);
			return this;
		}
	}
	
}