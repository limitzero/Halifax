using System;
using Halifax.Bus.Eventing;
using Halifax.Storage.Internals;

namespace Halifax.Bus.Eventing
{
    /// <summary>
    /// Contract for event bus that can be started for relaying event messages
    /// from the domain to the appropriate handlers for storage.
    /// </summary>
    public interface IStartableEventBus : IStartable, IEventBus, IDisposable
    {
    }
}