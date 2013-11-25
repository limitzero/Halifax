using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel;
using Halifax.Commands;
using Halifax.Configuration;
using Halifax.Domain;
using Halifax.Events;
using Halifax.Internals.Exceptions;
using Halifax.StateMachine;
using Halifax.StateMachine.Impl;

namespace Halifax.Internals.Reflection
{
	public class DefaultReflection : IReflection
	{
		private readonly IContainer container;

		public DefaultReflection(IContainer container)
		{
			this.container = container;
		}

		#region IReflection Members

		public object BuildInstance(Type currentType)
		{
			object instance = null;

			try
			{
				instance = currentType.Assembly.CreateInstance(currentType.FullName);
			}
			catch (Exception)
			{
				string msg = string.Format("Could create the instance from the assembly '{0}' to create type '{1}'.",
										   currentType.Assembly.FullName,
										   currentType.FullName);
				throw;
			}

			return instance;
		}

		public object BuildInstance(string typeName)
		{
			object instance = null;
			Assembly asm = null;

			string[] typeParts = typeName.Split(new[] { ',' });

			try
			{
				asm = Assembly.Load(typeParts[1]);
			}
			catch
			{
				string msg = string.Format("Could not load the assembly {0} to create type {1}.", typeParts[1],
										   typeParts[0]);

				//m_logger.Error(msg, exception);
				return instance;
			}

			try
			{
				instance = asm.CreateInstance(typeParts[0]);
			}
			catch
			{
				string msg = string.Format("Could not create the type {0}.", typeParts[0]);
				//m_logger.Error(msg, exception);
				return instance;
			}

			return instance;
		}

		public Type FindConcreteTypeImplementingInterface(Type interfaceType, Assembly assemblyToScan)
		{
			Type retval = null;

			foreach (Type type in assemblyToScan.GetTypes())
			{
				if (type.IsClass & !type.IsAbstract)
					if (interfaceType.IsAssignableFrom(type))
					{
						retval = type;
						break;
					}
			}

			return retval;
		}

		public Type[] FindConcreteTypesImplementingInterface(Type interfaceType, Assembly assemblyToScan)
		{
			var retval = new List<Type>();

			try
			{
				foreach (Type type in assemblyToScan.GetTypes())
				{
					if (type.IsClass & !type.IsAbstract)
						if (interfaceType.IsAssignableFrom(type))
						{
							if (!retval.Contains(type))
								retval.Add(type);
						}
				}
			}
			catch
			{
			}

			return retval.ToArray();
		}

		public object[] FindConcreteTypesImplementingInterfaceAndBuild(Type interfaceType, Assembly assemblyToScan)
		{
			var objects = new List<object>();
			Type[] types = FindConcreteTypesImplementingInterface(interfaceType, assemblyToScan);

			foreach (Type type in types)
			{
				if (type.IsAbstract) continue;
				objects.Add(BuildInstance(type.AssemblyQualifiedName));
			}

			return objects.ToArray();
		}

		public void InvokeValidateMethodForCommandValidator(Command command)
		{
			Type command_validator_type = typeof(CommandInputValidator.For<>).MakeGenericType(command.GetType());

			IEnumerable<object> theHandlers = container.ResolveAll(command_validator_type);

			if (theHandlers.Count() == 0)
				return;

			if (theHandlers.Count() > 1)
				throw new MultipleCommandValidatorsFoundForCommandException(command);

			object command_validator = theHandlers.First();

			Delegate action = Delegate.CreateDelegate(typeof(Action<>)
														.MakeGenericType(new Type[] { command.GetType() }), command_validator,
													  "Validate");
			action.DynamicInvoke(command);
		}

		public AggregateRoot InvokeExecuteMethodForCommandConsumer(Command command)
		{
			AggregateRoot aggregate_root = null;
			Type theType = typeof(CommandConsumer.For<>).MakeGenericType(command.GetType());
			IEnumerable<object> theHandlers;
			object theHandler;

			try
			{
				theHandlers = container.ResolveAll(theType);
				IEnumerator iter = theHandlers.GetEnumerator();
				iter.MoveNext();
				theHandler = iter.Current;

				if (theHandlers.Count() > 1)
					throw new MultipleCommandHandlersFoundForCommandException(command);
			}
			catch
			{
				var cex = new UnRegisteredCommandHandlerForCommandException(command.GetType());
				throw cex;
			}

			aggregate_root = theHandler.GetType().InvokeMember("Execute", BindingFlags.InvokeMethod, null, theHandler,
											  new object[] { command }) as AggregateRoot;

			if (aggregate_root != null)
			{
				aggregate_root.CurrentCommand = command;
			}

			return aggregate_root;
		}

		public void InvokeHandleMethodForEventConsumer(Event @event)
		{
			IEnumerable<object> event_handlers;
			Type event_consumer_type = typeof(EventConsumer.For<>).MakeGenericType(@event.GetType());

			try
			{
				event_handlers = container.ResolveAll(event_consumer_type);
			}
			catch (Exception)
			{
				var mex = new MissingExternalEventHandlerForEventException(@event.GetType());
				throw mex;
			}

			foreach (object event_handler in event_handlers)
			{
				try
				{
					if (typeof(IStateMachine).IsAssignableFrom(@event_handler.GetType()))
					{
						var state_machine_executer = this.container.Resolve<StateMachineExecuter>();
						state_machine_executer.Execute(event_handler as IStateMachine, @event);
					}
					else
					{
						Delegate action = Delegate.CreateDelegate(typeof (Action<>).MakeGenericType(new Type[] {@event.GetType()}),
						                                          event_handler, "Handle");
						action.DynamicInvoke(@event);
					}
				}
				catch
				{
					throw;
				}
			}
		}

		#endregion
	}
}