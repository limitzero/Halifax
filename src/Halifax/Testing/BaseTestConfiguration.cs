using System;

namespace Halifax.Testing
{
	public abstract class BaseTestConfiguration : IDisposable
	{
		protected static Halifax.Configuration.Impl.Configuration Configuration { get; private set; }

		protected BaseTestConfiguration()
		{
			Configuration = new Configuration.Impl.Configuration();
		}

		public virtual void Dispose()
		{
			if (Configuration != null)
			{
				Configuration.Dispose();
			}
			Configuration = null;
			GC.SuppressFinalize(this);
		}

		protected virtual void BuildInfrastructure()
		{
			Configuration
				.Container (c => c.UsingCastleWindsor())
				.Eventing(ev => ev.Synchronous())
				.EventStore(es => es.UsingInMemoryStorage())
				.Serialization(s =>s.UsingSharpSerializer())
				.ReadModel (rm => rm.UsingInMemoryRepository())
				.Configure(this.GetType().Assembly);
		}
	}
}