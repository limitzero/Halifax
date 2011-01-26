using System;

namespace Halifax.Bus.Eventing.Async.Subscriptions
{
    public interface ISubscriptionManager
    {
        void RegisterSubscription(string location, Type message);
        ISubscription[] GetSubscriptions(object message);
    }
}