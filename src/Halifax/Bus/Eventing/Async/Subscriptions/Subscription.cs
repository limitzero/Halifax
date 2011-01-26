namespace Halifax.Bus.Eventing.Async.Subscriptions
{
    public class Subscription : ISubscription
    {
        #region ISubscription Members

        public string Location { get; set; }
        public string Message { get; set; }

        #endregion
    }
}