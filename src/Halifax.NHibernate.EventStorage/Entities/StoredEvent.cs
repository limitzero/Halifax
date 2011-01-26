using System;
using Halifax.Storage.Events;

namespace Halifax.NHibernate.EventStorage.Entities
{
    /// <summary>
    /// Created to keep the mapping and the mapped entity in the same 
    /// assembly for consistency.
    /// </summary>
    [Serializable]
    public class StoredEvent : PersistableDomainEvent
    {
    }
}