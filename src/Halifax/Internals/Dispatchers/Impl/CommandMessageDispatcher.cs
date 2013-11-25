using System;
using Halifax.Commands;
using Halifax.Configuration;
using Halifax.Domain;
using Halifax.Internals.Reflection;

namespace Halifax.Internals.Dispatchers.Impl
{
	public class CommandMessageDispatcher :
		ICommandMessageDispatcher
	{
		private readonly IContainer container;
		private readonly IReflection reflection;

		public CommandMessageDispatcher(IContainer container, IReflection reflection)
		{
			this.container = container;
			this.reflection = reflection;
		}

		public void Dispatch(Command command)
		{
			try
			{
				ExecuteCommandValidatorFor(command);
				ExecuteCommandConsumerFor(command);
			}
			catch (Exception e)
			{
				Exception toThrow = e;

				while (e != null)
				{
					toThrow = e;
					e = e.InnerException;
				}

				throw toThrow;
			}
		}

		private void ExecuteCommandValidatorFor(Command command)
		{
			reflection.InvokeValidateMethodForCommandValidator(command);
		}

		private void ExecuteCommandConsumerFor(Command command)
		{
			command.At = System.DateTime.Now;
			AggregateRoot aggregate_root = reflection.InvokeExecuteMethodForCommandConsumer(command);
			if (aggregate_root == null) return;

			var unit_of_work = this.container.Resolve<IUnitOfWork>();
			unit_of_work.Accept(aggregate_root);
		}
	}
}