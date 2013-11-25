using NHibernate;

namespace Halifax.NHibernate
{
    public class NHibernateSession : INHibernateSession
    {
        public ISession Session { get; set; }
    }
}