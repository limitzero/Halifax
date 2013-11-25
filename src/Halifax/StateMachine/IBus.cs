using Halifax.Commands;
using Halifax.Events;

namespace Halifax.StateMachine
{
	public interface IBus
	{
		void Publish(params Event[] events);
		void Send(params Command[] commands);
	}
}