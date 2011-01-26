namespace Halifax.Eventing
{
    public interface IEventMessageDispatcher
    {
        void Dispatch<TEVENT>(TEVENT domainEvent)
            where TEVENT : class, IDomainEvent;
    }
}