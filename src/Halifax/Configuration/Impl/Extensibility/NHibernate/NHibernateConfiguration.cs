using System.Collections.Generic;
using System.Reflection;

namespace Halifax.Configuration.Impl.Extensibility.NHibernate
{
	public class NHibernateConfiguration
	{
		private readonly IDictionary<string, string> properties;

		public Assembly MappingAssembly { get; set; }

		public NHibernateConfiguration()
		{
			properties = new Dictionary<string, string>();
		}

		public NHibernateConfiguration WithConnectionStringOf(string connection)
		{
			properties.Add("connection.connection_string", connection);
			return this;
		}

		public NHibernateConfiguration WithMappingAssemblyOf(Assembly assembly)
		{
			this.MappingAssembly = assembly;
			return this;
		}

		public NHibernateConfiguration WithSql2008ServerDialect()
		{
			this.properties.Remove("dialect");
			this.properties.Add("dialect", "NHibernate.Dialect.MsSql2008Dialect");
			return this;
		}

		public NHibernateConfiguration WithSql2005ServerDialect()
		{
			this.properties.Remove("dialect");
			this.properties.Add("dialect", "NHibernate.Dialect.MsSql2005Dialect");
			return this;
		}

		public NHibernateConfiguration WithSqlClientDriverClass()
		{
			this.properties.Remove("connection.driver_class");
			this.properties.Add("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
			return this;
		}

		public NHibernateConfiguration WithSqlCEDriverClass()
		{
			this.properties.Remove("connection.driver_class");
			this.properties.Add("connection.driver_class", "NHibernate.Driver.SqlServerCeDriver");
			return this;
		}

		public IDictionary<string, string> GetProperties()
		{
			properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			//properties.Add("connection.driver_class", "NHibernate.Driver.SqlServerCeDriver");
			//properties.Add("dialect", "NHibernate.Dialect.MsSqlCeDialect");
			//properties.Add("connection.connection_string", this.connection);
			properties.Add("connection.release_mode", "on_close");
			properties.Add("show_sql", "true");
			properties.Add("use_outer_join", "true");
			properties.Add("command_timeout", "444");
			properties.Add("query.substitutions", "true 1, false 0, yes 1, no 0");
			properties.Add("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
			return properties;
		}
	}
}