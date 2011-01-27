using System;
using Halifax.Commanding;
using Halifax.Internals.Reflection;

namespace Halifax.Internals.Dispatchers
{
    public class CommandMessageDispatcher :
        ICommandMessageDispatcher
    {
        private readonly IReflection _reflection;
        private readonly IUnitOfWork _session;

        public CommandMessageDispatcher(IReflection reflection,
                                        IUnitOfWork session)
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