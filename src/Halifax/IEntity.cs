using System;

namespace Halifax
{
    /// <summary>
    /// Contract for all object acting as entities.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// (Read-Write). The current identifier for the unique instance of an entity.
        /// </summary>
        Guid Id { get; set; }
    }
}