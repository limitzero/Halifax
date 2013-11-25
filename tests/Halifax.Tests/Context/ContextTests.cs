using System;
using System.Threading;
using Halifax.Commands;
using Halifax.Domain;
using Xunit;

namespace Halifax.Tests.Context
{
	public class when_using_the_central_context
	{
		private static Command issued_command;

		public when_using_the_central_context()
		{
			HalifaxContext.ConfigurationFactory
				.ConfigureWith()
					.Container(c => c.UsingCastleWindsor())
					.Eventing(ev => ev.Synchronous())
					.EventStore(es => es.UsingInMemoryStorage())
					.Serialization(s => s.UsingJSON())
					.ReadModel(rm => rm.UsingInMemoryRepository())
					.Configure(this.GetType().Assembly);
		}

		[Fact]
		public void it_should_send_a_command_to_the_command_handler()
		{
			HalifaxContext.ConfigurationFactory.SendCommand(new ContextCommand());
			Assert.IsType<ContextCommand>(issued_command);
		}

		public class ContextCommand : Command{}

		public class ContextCommandConsumer : CommandConsumer.For<ContextCommand>
		{
			public override AggregateRoot Execute(ContextCommand command)
			{
				issued_command = command;
				return null;
			}
		}

	}
}