using NHibernate;

namespace Halifax.NHibernate
{
    public interface INHibernateSession
    {
        ISession Session { get; set; }
    }
}