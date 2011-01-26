using NHibernate;

namespace Halifax.NHibernate.AggregateStorage
{
    public class AggregateStorageSession : IAggregateStorageSession
    {
        public ISession Session { get; set; }
    }
}