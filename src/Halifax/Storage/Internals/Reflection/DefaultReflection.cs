using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel;
using Halifax.Commanding;
using Halifax.Eventing;
using Halifax.Exceptions;
using Halifax.Storage.Internals.Reflection;

namespace Halifax.Storage.Internals.Reflection
{
    public class DefaultReflection : IReflection
    {
        private readonly IKernel _kernel;

        public DefaultReflection(IKernel kernel)
        {
            _kernel = kernel;
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

            string[] typeParts = typeName.Split(new[] {','});

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

        public void InvokeExecuteMethodForCommandConsumer<TCOMMAND>(IUnitOfWorkSession session, TCOMMAND command) 
            where TCOMMAND : Command
        {
            Type theType = typeof(CommandConsumer.For<>).MakeGenericType(typeof(TCOMMAND));
            CommandConsumer.For<TCOMMAND> theHandler;

            try
            {
                Array theHandlers = _kernel.ResolveAll(theType);
                IEnumerator iter = theHandlers.GetEnumerator();
                iter.MoveNext();
                
                if (theHandlers.Length > 1)
                    throw new MultipleCommandHandlersFoundForCommandException(command);

                theHandler = _kernel.Resolve(theType) as CommandConsumer.For<TCOMMAND>;
            }
            catch (Exception e)
            {
                var cex = new UnRegisteredCommandHandlerForCommandException(command.GetType());
                throw cex;
            }

            theHandler.Execute(session, command);
        }

        public void InvokeExecuteMethodForCommandHandler(Command command)
        {
            Type theType = typeof (CommandConsumer.For<>).MakeGenericType(command.GetType());
            Array theHandlers;
            object theHandler;

            try
            {
                theHandlers = _kernel.ResolveAll(theType);
                IEnumerator iter = theHandlers.GetEnumerator();
                iter.MoveNext();
                theHandler = iter.Current;

                if (theHandlers.Length > 1)
                    throw new MultipleCommandHandlersFoundForCommandException(command);
            }
            catch (Exception e)
            {
                var cex = new UnRegisteredCommandHandlerForCommandException(command.GetType());
                throw cex;
            }

            theHandler.GetType().InvokeMember("Execute", BindingFlags.InvokeMethod, null, theHandler,
                                              new object[] {command});
        }

        public void InvokeExecuteMethodForCommandConsumer(IUnitOfWorkSession session, Command command)
        {
            Type theType = typeof (CommandConsumer.For<>).MakeGenericType(command.GetType());
            Array theHandlers;
            object theHandler;

            try
            {
                theHandlers = _kernel.ResolveAll(theType);
                IEnumerator iter = theHandlers.GetEnumerator();
                iter.MoveNext();
                theHandler = iter.Current;

                if (theHandlers.Length > 1)
                    throw new MultipleCommandHandlersFoundForCommandException(command);
            }
            catch (Exception e)
            {
                var cex = new UnRegisteredCommandHandlerForCommandException(command.GetType());
                throw cex;
            }

            session.CurrentCommand = command;
            theHandler.GetType().InvokeMember("Execute", BindingFlags.InvokeMethod, null, theHandler,
                                              new object[] {session, command});
        }

        public void InvokeHandleMethodForEventConsumer(IDomainEvent domainEvent)
        {
            Type theType = typeof (EventConsumer.For<>).MakeGenericType(domainEvent.GetType());
            Array theHandlers;

            try
            {
                theHandlers = _kernel.ResolveAll(theType);
            }
            catch (Exception)
            {
                var mex = new MissingExternalEventHandlerForEventException(domainEvent.GetType());
                throw mex;
            }

            foreach (object theHandler in theHandlers)
            {
                try
                {
                    Delegate action = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(new Type[] {domainEvent.GetType()}), theHandler, "Handle");
                    action.DynamicInvoke(domainEvent);

                    //theHandler.GetType().InvokeMember("Handle", BindingFlags.InvokeMethod, null, theHandler,
                    //                                  new object[] {domainEvent});
                }
                catch (Exception e)
                {
                }
            }
        }

        #endregion
    }
}