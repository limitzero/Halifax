namespace Halifax.Configuration
{
	/// <summary>
	/// Interface for a concrete class to configure the options for the framework:
	/// </summary>
	public interface IConfigurator
	{
		void Configure(IConfiguration configuration);
	}
}