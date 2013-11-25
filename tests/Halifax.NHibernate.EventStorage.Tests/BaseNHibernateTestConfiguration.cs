using System;

namespace Halifax.NHibernate.Tests
{
	public abstract class BaseNHibernateTestConfiguration : IDisposable
	{
		protected Halifax.Configuration.Impl.Configuration Configuration { get; private set; }

		protected BaseNHibernateTestConfiguration()
		{
			this.Configuration = new Configuration.Impl.Configuration();
		}

		public virtual void Dispose()
		{
			if (this.Configuration != null)
			{
				this.Configuration.Dispose();
			}
			this.Configuration = null;
		}

		protected virtual void BuildInfrastructure()
		{
			this.Configuration
				.Container(c => c.UsingCastleWindsor())
				.Eventing( ev => ev.Synchronous())
				.EventStore(ev => ev.UsingNHibernate(nh => 
					nh.WithConnectionStringOf(@"Data Source=.\SqlExpress;Initial Catalog=contoso;Integrated Security=True")
					.WithSql2008ServerDialect()
					.WithSqlClientDriverClass()
					.WithMappingAssemblyOf(typeof(Halifax.NHibernate.NHibernateSession).Assembly)))
				.Serialization(srl => srl.UsingJSON())
				.ReadModel( rm => 
					    rm.UsingNHibernate(nh => 
							nh.WithConnectionStringOf(@"Data Source=.\SqlExpress;Initial Catalog=contoso;Integrated Security=True")
						.WithSql2008ServerDialect()
						.WithMappingAssemblyOf(this.GetType().Assembly)))
				// add the Halifax.NHibernate assembly for configuration as well to pick up the configuration in the assembly
				.Configure(this.GetType().Assembly,
							 typeof(Halifax.NHibernate.NHibernateSession).Assembly);
		}
	}
}