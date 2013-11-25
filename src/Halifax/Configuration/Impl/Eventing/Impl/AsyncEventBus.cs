using System.Threading.Tasks;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Domain;
using Halifax.Internals.Dispatchers;

namespace Halifax.Configuration.Impl.Eventing.Impl
{
	/// <summary>
	/// Event bus that processes the events from the domain entities
	/// in an asynchronous fashion.
	/// </summary>
	public class AsyncEventBus : InProcessEventBus
	{
		public AsyncEventBus(IContainer container, IEventMessageDispatcher dispatcher) : 
			base(container, dispatcher)
		{
		}

		public override void Publish<TEVENT>(params TEVENT[] @events)
		{
			Task.Factory.StartNew(() => Publish(@events));
		}

		public override void Publish(AggregateRoot root)
		{
			Task.Factory.StartNew(() => Publish(root));
		}
	}
}