using System;

namespace Halifax.Commanding
{
    /// <summary>
    /// Contract for all commands that are issued by the caller to the domain for aggregate mutation.
    /// </summary>
    [Serializable]
    public abstract class Command : IMessage
    {
    }
}