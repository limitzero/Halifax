using System;
using Halifax.Commanding;
using Halifax.Storage.Internals.Reflection;

namespace Halifax.Storage.Internals.Dispatchers
{
    public class CommandMessageDispatcher :
        ICommandMessageDispatcher
    {
        private readonly IReflection _reflection;
        private readonly IUnitOfWorkSession _session;

        public CommandMessageDispatcher(IReflection reflection,
                                        IUnitOfWorkSession session)
        {
            _reflection = reflection;
            _session = session;
        }

        public void Dispatch<TCommand>(TCommand command)
            where TCommand : Command
        {
            try
            {
                _reflection.InvokeExecuteMethodForCommandConsumer(_session, command);
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
    }
}