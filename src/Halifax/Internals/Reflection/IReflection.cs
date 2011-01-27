using System;
using System.Reflection;
using Halifax.Commanding;
using Halifax.Eventing;

namespace Halifax.Internals.Reflection
{
    public interface IReflection
    {
        void InvokeExecuteMethodForCommandHandler(Command command);

        void InvokeExecuteMethodForCommandConsumer(IUnitOfWork session, Command command);
        void InvokeHandleMethodForEventConsumer(IDomainEvent domainEvent);

        object BuildInstance(Type currentType);
        object BuildInstance(string typeName);

        Type FindConcreteTypeImplementingInterface(Type interfaceType, Assembly assemblyToScan);
        Type[] FindConcreteTypesImplementingInterface(Type interfaceType, Assembly assemblyToScan);

        object[] FindConcreteTypesImplementingInterfaceAndBuild(Type interfaceType, Assembly assemblyToScan);
    }
}