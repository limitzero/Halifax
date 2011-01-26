using NHibernate;

namespace Halifax.NHibernate.AggregateStorage
{
    public interface IAggregateStorageSession
    {
        ISession Session { get; set; }
    }
}