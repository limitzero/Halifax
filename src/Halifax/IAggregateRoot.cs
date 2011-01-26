namespace Halifax
{
    /// <summary>
    /// Contract for any aggregate root.
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
        int Version { get; set; }
    }
}