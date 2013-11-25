using System;
using Halifax.Configuration.Impl.Eventing.Impl;

namespace Halifax.Configuration.Impl.Eventing
{
	public class EventingOptions
	{
		private readonly IContainer container;

		public EventingOptions(IContainer container)
		{
			this.container = container;
		}

		/// <summary>
		/// This will dispatch the events from the aggregate root out-of-process with the changes that are made.
		/// </summary>
		/// <returns></returns>
		public EventingOptions Asynchronous()
		{
			this.container.Register<IEventBus, AsyncEventBus>();
			return this;
		}

		/// <summary>
		/// This will dispatch the events from the aggregate root out-process with the changes that are made
		/// via an external component for dispatching events to components that will process those events.
		/// </summary>
		/// <returns></returns>
		public EventingOptions AsynchronousVia(Func<IContainer, IEventBus> option)
		{
			var eventing_bus = option(this.container);
			this.container.Register(typeof(IEventBus), eventing_bus.GetType());
			return this;
		}

		/// <summary>
		/// This will dispatch the events from the aggregate root in-process with the changes that are made.
		/// </summary>
		/// <returns></returns>
		public EventingOptions Synchronous()
		{
			this.container.Register<IEventBus, InProcessEventBus>();
			return this;
		}

	}
}