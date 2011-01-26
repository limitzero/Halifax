using System;
using Halifax.Storage.Internals;

namespace Halifax.Bus.Commanding
{
    /// <summary>
    /// Contract for command bus that can be started for relaying command messages
    /// to the domain for processing.
    /// </summary>
    public interface IStartableCommandBus : IStartable, ICommandBus, IDisposable
    {
    }
}