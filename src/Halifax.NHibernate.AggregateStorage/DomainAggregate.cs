using System;

namespace Halifax.NHibernate.AggregateStorage
{
    [Serializable]
    public class DomainAggregate
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Version { get; set; }
        public virtual byte[] Data { get; set; }
        public virtual DateTime Timestamp { get; set; }
    }
}
