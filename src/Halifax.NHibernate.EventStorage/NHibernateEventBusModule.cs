using System;
using Halifax.Events;
using Halifax.Events.Module;
using NHibernate;

namespace Halifax.NHibernate
{
    public class NHibernateEventBusModule :
        AbstractEventBusModule
    {
        private readonly ISessionFactory _factory;

        [ThreadStatic]
        private static INHibernateSession _currentsession;

        public NHibernateEventBusModule(ISessionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Factory method for creating sessions.
        /// </summary>
        /// <returns></returns>
        public INHibernateSession GetCurrentSession()
        {
            CreateSession();
            return _currentsession;
        }

        public override void OnEventBusStartMessagePublishing(EventBusStartPublishMessageEventArgs args)
        {
            // always try to get a new session when the bus begins publishing:
            CreateSession();
        }

        public override void OnEventBusCompletedMessagePublishing(EventBusCompletedPublishMessageEventArgs args)
        {
            try
            {
                if (_currentsession == null) return;
                
                if(_currentsession.Session == null) return;

                _currentsession.Session.Dispose();
                _currentsession.Session = null;
                _currentsession = null;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void CreateSession()
        {
            _currentsession = new NHibernateSession {Session = _factory.OpenSession()};
        }
    }
}