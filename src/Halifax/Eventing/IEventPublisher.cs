using System;
using Halifax.Bus.Eventing;

namespace Halifax.Eventing
{
    [Obsolete]
    public interface IEventPublisher
    {
        void SetEventPublisher(IEventBus publisher);
    }
}