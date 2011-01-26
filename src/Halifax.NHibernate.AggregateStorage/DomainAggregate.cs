using System;

namespace Halifax.NHibernate.AggregateStorage
{
    [Serializable]
    public class DomainAggregate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public byte[] Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
