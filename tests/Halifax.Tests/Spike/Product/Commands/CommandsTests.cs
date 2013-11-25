using System;
using Moq;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Halifax.Tests.Spike.Product.Commands
{
	public class commanding_concern : IDisposable
	{
		private bool disposed;

		protected static TestBus subject_under_test;
		protected static SampleCommand current_command;
		protected static Mock<ICommandHandlerResolver> command_handler_resolver;
		protected static Mock<ICommandDispatcher> command_dispatcher;

		public void Dispose()
		{
			Disposing(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Disposing(bool disposing)
		{
			if(disposing == true)
			{
				subject_under_test = null;
				current_command = null;
				command_handler_resolver = null;
				command_dispatcher = null;
				context = null;
			}
			this.disposed = true;
		}

		Establish context = () =>
		{
			command_handler_resolver = new Mock<ICommandHandlerResolver>(MockBehavior.Loose);
			command_dispatcher = new Mock<ICommandDispatcher>(MockBehavior.Loose);

			subject_under_test = new TestBus(command_handler_resolver.Object, command_dispatcher.Object);
			current_command = new SampleCommand();
		};
	}

	[Subject("sending command with no handler for a command")]
	public class when_sending_a_command_with_no_handler_registered : commanding_concern
	{
		Because of = () =>
								{
									ICommandHandler a_handler = null;
									command_handler_resolver.Setup(r => r.Resolve(current_command))
										.Returns(a_handler);

									exception = Catch.Exception(() => subject_under_test.Send(current_command));
								};

		It should_call_the_command_handler_resolver_to_find_the_designated_handler_for_the_command = () =>
			command_handler_resolver.Verify(r => r.Resolve(current_command), Times.AtLeastOnce());

		It should_raise_an_exception_noting_that_the_there_is_not_a_command_handler_for_the_command = () => exception.ShouldNotBeNull();

		private static Exception exception;
	}

	[Subject("sending command with  handler for a command")]
	public class when_sending_a_command_with_handler_registered : commanding_concern
	{
		Because of = () =>
								{
									handler_stub = new Mock<ICommandHandler<SampleCommand>>().Object;
									command_handler_resolver.Setup(r => r.Resolve(current_command))
										.Returns(handler_stub);

									command_dispatcher.Setup(d => d.Dispatch(handler_stub, current_command));

									subject_under_test.Send(current_command);
								};

		It should_call_the_command_handler_resolver_to_find_the_designated_handler_for_the_command = () =>
			command_handler_resolver.Verify(r => r.Resolve(current_command), Times.AtLeastOnce());

		It should_invoke_the_dispatcher_to_call_the_handler_for_the_given_command = () =>
			command_dispatcher.Verify( d => d.Dispatch(handler_stub, current_command));

		static Exception exception;
		static ICommandHandler<SampleCommand> handler_stub;
	}

	// marker:
	public abstract class Command
	{ }

	public class SampleCommand : Command
	{ }

	public interface ICommandBus
	{
		void Send(Command command);
	}

	public class TestBus : ICommandBus
	{
		private readonly ICommandHandlerResolver command_handler_resolver;
		private readonly ICommandDispatcher command_dispatcher;

		public TestBus(ICommandHandlerResolver commandHandlerResolver, ICommandDispatcher commandDispatcher)
		{
			command_handler_resolver = commandHandlerResolver;
			command_dispatcher = commandDispatcher;
		}

		public void Send(Command command)
		{
			var handler = this.command_handler_resolver.Resolve(command);

			if (handler == null)
				throw new InvalidOperationException(string.Format("There was not a handler defined for command '{0}'",
					command.GetType().FullName));

			this.command_dispatcher.Dispatch(handler, command);
		}
	}

	// marker interface:
	public interface ICommandHandler
	{ }

	public interface ICommandHandler<TCommand> : ICommandHandler where TCommand : Command
	{
		void Handle(TCommand command);
	}

	public interface ICommandHandlerResolver
	{
		ICommandHandler Resolve(Command command);
	}

	public interface ICommandDispatcher
	{
		bool Dispatch(ICommandHandler handler, Command command);
	}
}