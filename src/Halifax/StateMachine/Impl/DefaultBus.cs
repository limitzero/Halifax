using System;
using Halifax.Commands;
using Halifax.Configuration.Impl.Eventing;
using Halifax.Events;
using Halifax.Internals.Commands;

namespace Halifax.StateMachine.Impl
{
	internal class DefaultBus : IBus
	{
		private readonly ICommandBus command_bus;
		private readonly IEventBus event_bus;

		public DefaultBus(ICommandBus commandBus, IEventBus eventBus)
		{
			command_bus = commandBus;
			event_bus = eventBus;
		}

		public void Publish(params Event[] events)
		{
			Array.ForEach(events, (@event) => this.event_bus.Publish<Event>(@event));
		}

		public void Send(params Command[] commands)
		{
			Array.ForEach(commands, (command) => this.command_bus.Send(command));
		}
	}
}