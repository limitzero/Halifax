using NHibernate;

namespace Halifax.NHibernate.EventStorage
{
    public class EventStorageSession : IEventStorageSession
    {
        public ISession Session { get; set; }
    }
}