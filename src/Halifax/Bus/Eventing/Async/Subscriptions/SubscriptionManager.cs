using System;
using System.Collections.Generic;
using System.Linq;

namespace Halifax.Bus.Eventing.Async.Subscriptions
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private static readonly object _subscriptions_lock = new object();
        private readonly List<ISubscription> _subscriptions;

        public SubscriptionManager()
        {
            if (_subscriptions == null)
                _subscriptions = new List<ISubscription>();
        }

        #region ISubscriptionManager Members

        public void RegisterSubscription(string location, Type message)
        {
            var subscription = new Subscription {Location = location, Message = message.FullName};

            if (!_subscriptions.Contains(subscription))
                lock (_subscriptions_lock)
                    _subscriptions.Add(subscription);
        }

        public ISubscription[] GetSubscriptions(object message)
        {
            ISubscription[] subscriptions = (from subscription in _subscriptions
                                             where subscription.Message == message.GetType().FullName
                                             select subscription).ToArray();
            return subscriptions;
        }

        #endregion
    }
}