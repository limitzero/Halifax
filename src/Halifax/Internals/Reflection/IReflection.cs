using System;
using System.Reflection;
using Halifax.Commands;
using Halifax.Domain;
using Halifax.Events;

namespace Halifax.Internals.Reflection
{
    public interface IReflection
    {
		/// <summary>
		/// This will execute the command-specific validator (if found) before handing 
		/// the message off to the command consumer for dispatching to the aggregate root.
		/// </summary>
		/// <param name="command"></param>
    	void InvokeValidateMethodForCommandValidator(Command command);

		/// <summary>
		/// This will execute the current command against the command handler and return the 
		/// aggregate root that was affected for extracting the resultant events that changed 
		/// the internal state of the aggregate root entity.
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
        AggregateRoot InvokeExecuteMethodForCommandConsumer(Command command);

		/// <summary>
		/// This will execute the event message against the event handler for persisting 
		/// changes out to other subscribers and also sending commands.
		/// </summary>
		/// <param name="domainEvent"></param>
        void InvokeHandleMethodForEventConsumer(Event domainEvent);

        object BuildInstance(Type currentType);
        object BuildInstance(string typeName);

        Type FindConcreteTypeImplementingInterface(Type interfaceType, Assembly assemblyToScan);
        Type[] FindConcreteTypesImplementingInterface(Type interfaceType, Assembly assemblyToScan);

        object[] FindConcreteTypesImplementingInterfaceAndBuild(Type interfaceType, Assembly assemblyToScan);
    }
}