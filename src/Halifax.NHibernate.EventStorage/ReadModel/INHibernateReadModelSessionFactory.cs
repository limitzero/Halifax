using NHibernate;

namespace Halifax.NHibernate.ReadModel
{
	public interface INHibernateReadModelSessionFactory
	{
		ISessionFactory Factory { get; set; }
		global::NHibernate.Cfg.Configuration Configuration { get; }
	}
}