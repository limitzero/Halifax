namespace Halifax.Bus.Eventing.Async.Subscriptions
{
    public interface ISubscription
    {
        string Location { get; set; }
        string Message { get; set; }
    }
}