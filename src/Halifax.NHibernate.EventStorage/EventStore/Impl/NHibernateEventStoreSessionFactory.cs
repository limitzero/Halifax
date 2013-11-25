using System;
using NHibernate;

namespace Halifax.NHibernate.EventStore.Impl
{
	public class NHibernateEventStoreSessionFactory : 
		INHibernateEventStoreSessionFactory
	{
		public NHibernateEventStoreSessionFactory(global::NHibernate.Cfg.Configuration configuration)
		{
			this.Configuration = configuration;
		}

		public ISessionFactory Factory { get; set; }

		public global::NHibernate.Cfg.Configuration Configuration { get; private set; }
	}
}