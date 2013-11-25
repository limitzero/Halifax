using Halifax.Commands;

namespace Halifax.Internals.Dispatchers
{
    public interface ICommandMessageDispatcher
    {
    	void Dispatch(Command command);
    }
}