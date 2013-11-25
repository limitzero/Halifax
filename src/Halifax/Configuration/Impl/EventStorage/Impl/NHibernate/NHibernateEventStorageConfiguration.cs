using Halifax.Configuration.Impl.Extensibility.NHibernate;

namespace Halifax.Configuration.Impl.EventStorage.Impl.NHibernate
{
	public class NHibernateEventStorageConfiguration 
	{
		public NHibernateConfiguration NHibernateConfiguration { get; set; }

		public NHibernateEventStorageConfiguration(NHibernateConfiguration nHibernateConfiguration)
		{
			NHibernateConfiguration = nHibernateConfiguration;
		}
	}
}