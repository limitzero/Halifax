namespace Halifax.Configuration
{
	/// <summary>
	/// Marker interface for external configuration of the underlying component container.
	/// </summary>
	public interface ICanConfigureContainer
	{
		void Configure(IContainer container);
	}
}