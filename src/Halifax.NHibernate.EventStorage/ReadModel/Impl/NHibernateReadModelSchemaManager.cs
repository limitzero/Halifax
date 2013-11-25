using NHibernate;

namespace Halifax.NHibernate.ReadModel.Impl
{
	public class NHibernateReadModelSchemaManager : INHibernateReadModelSchemaManager
	{
		private readonly INHibernateReadModelSessionFactory session_factory;

		public NHibernateReadModelSchemaManager(INHibernateReadModelSessionFactory sessionFactory)
		{
			session_factory = sessionFactory;
		}

		public ISessionFactory GetSessionFactory()
		{
			return this.session_factory.Factory;
		}

		public void CreateSchema()
		{
			var exporter = new global::NHibernate.Tool.hbm2ddl.SchemaExport(session_factory.Configuration);
			exporter.Execute(true, true, false);
		}

		public void DropSchema()
		{
			var exporter = new global::NHibernate.Tool.hbm2ddl.SchemaExport(session_factory.Configuration);
			exporter.Execute(true, true, true);
		}
	}
}