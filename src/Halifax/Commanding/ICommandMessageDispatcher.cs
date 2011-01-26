namespace Halifax.Commanding
{
    public interface ICommandMessageDispatcher
    {
        void Dispatch<TCommand>(TCommand command)
            where TCommand : Command;
    }
}