using System;
using Halifax.Configuration.Impl.Extensibility.NHibernate;
using Halifax.Configuration.Impl.Repository.Impl;
using Halifax.Read;

namespace Halifax.Configuration.Impl.Repository
{
	/// <summary>
	///  Marker for the options to configure the read model for storage and retrieval.
	/// </summary>
	public class ReadModelOptions
	{
		private readonly IContainer container;

		public ReadModelOptions(IContainer container)
		{
			this.container = container;
		}

		/// <summary>
		/// Use the in-memory (volatile) storage and persistance for read model data projected 
		/// from the aggregate root behavioral model via events
		/// </summary>
		/// <returns></returns>
		public ReadModelOptions UsingInMemoryRepository()
		{
			this.container
				.Register(typeof(IReadModelRepository<>),
				typeof(InMemoryReadModelRepository<>));
			return this;
		}

		/// <summary>
		/// Use NHibernate storage and persistance for read model data projected from the aggregate root behavioral model via events
		/// (each read model projection or class must have the appropriate mapping file per normal operations for NHibernate)
		/// </summary>
		/// <returns></returns>
		public ReadModelOptions UsingNHibernate(Func<NHibernateConfiguration, NHibernateConfiguration> options)
		{
			var nhibernate_configuration = options(new NHibernateConfiguration());
			var read_model = new NHibernateReadModelConfiguration(nhibernate_configuration);
			this.container.RegisterInstance<NHibernateReadModelConfiguration>(read_model);
			return this;
		}

		/// <summary>
		/// This will tell the configuration that while storing and retrieving read model data to the persistance store, a custom implemenation 
		/// will be in use that is outside of the normal read model configuration options.
		/// </summary>
		/// <returns></returns>
		public ReadModelOptions UsingCustomPersistance()
		{
			return this;
		}

	}
}