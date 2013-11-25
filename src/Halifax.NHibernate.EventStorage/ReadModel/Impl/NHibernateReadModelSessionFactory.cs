using System;
using NHibernate;

namespace Halifax.NHibernate.ReadModel.Impl
{
	public class NHibernateReadModelSessionFactory : INHibernateReadModelSessionFactory
	{
		public NHibernateReadModelSessionFactory(global::NHibernate.Cfg.Configuration configuration)
		{
			this.Configuration = configuration;
		}

		public ISessionFactory Factory { get; set; }

		public global::NHibernate.Cfg.Configuration Configuration { get; private set; }
	}
}