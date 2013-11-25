using Halifax.Events;

namespace Halifax.Internals.Dispatchers
{
    public interface IEventMessageDispatcher
    {
    	void Dispatch(Event @event);
    }
}