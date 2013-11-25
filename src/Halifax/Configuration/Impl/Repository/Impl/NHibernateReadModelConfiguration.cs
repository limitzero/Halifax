using Halifax.Configuration.Impl.Extensibility.NHibernate;

namespace Halifax.Configuration.Impl.Repository.Impl
{
	public class NHibernateReadModelConfiguration
	{
		public NHibernateConfiguration NHibernateConfiguration { get; set; }

		public NHibernateReadModelConfiguration(NHibernateConfiguration nHibernateConfiguration)
		{
			NHibernateConfiguration = nHibernateConfiguration;
		}
	}
}