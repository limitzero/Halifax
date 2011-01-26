namespace Halifax.Commanding
{
    public interface IMessageDispatcher
    {
        void Dispatch(object message, object handler);
    }
}