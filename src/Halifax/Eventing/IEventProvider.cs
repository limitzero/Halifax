using System.Collections.Generic;

namespace Halifax.Eventing
{
    public interface IEventProvider
    {
        void Clear();
        void Record(IDomainEvent domainEvent);
        ICollection<IDomainEvent> GetChanges();
        void LoadFromHistory(IEnumerable<IDomainEvent> domainEvents);
    }
}