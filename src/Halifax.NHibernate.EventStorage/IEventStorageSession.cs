using System;
using NHibernate;

namespace Halifax.NHibernate.EventStorage
{
    public interface IEventStorageSession
    {
        ISession Session { get; set; }
    }
}