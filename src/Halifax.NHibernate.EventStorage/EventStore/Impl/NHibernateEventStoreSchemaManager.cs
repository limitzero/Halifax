using NHibernate;

namespace Halifax.NHibernate.EventStore.Impl
{
	public class NHibernateEventStoreSchemaManager : INHibernateEventStoreSchemaManager
	{
		private readonly INHibernateEventStoreSessionFactory session_factory;

		public NHibernateEventStoreSchemaManager(INHibernateEventStoreSessionFactory sessionFactory)
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