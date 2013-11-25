namespace Halifax.Domain
{
    /// <summary>
    /// Contract for any aggregate root.
    /// </summary>
    public interface IAggregateRoot
    {
        int Version { get; set; }
    }
}