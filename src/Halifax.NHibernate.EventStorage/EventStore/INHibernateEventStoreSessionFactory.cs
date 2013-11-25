using NHibernate;

namespace Halifax.NHibernate.EventStore
{
	public interface INHibernateEventStoreSessionFactory
	{
		ISessionFactory Factory { get; set; }
		global::NHibernate.Cfg.Configuration Configuration { get; }
	}
}