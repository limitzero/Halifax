using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Halifax.Storage.Aggregates;
using Halifax.Storage.Events;

namespace Halifax.Testing
{
    /// <summary>
    /// Test fixture for testing the functionality of the entities that represent aggregate roots.
    /// </summary>
    /// <typeparam name="TAGGREGATE">The type of the object acting as the aggregate root</typeparam>
    public abstract class BaseAggregateTestFixture<TAGGREGATE>
        where TAGGREGATE : AbstractAggregateRoot,  new()
    {
        private IWindsorContainer _container;

        /// <summary>
        /// (Read-Only). The current instance of the aggregate under test.
        /// </summary>
        public TAGGREGATE Aggregate { get; private set; }

        /// <summary>
        /// (Read-Only). The currently caught exception as captured for 
        /// sending the command to the domain for state change.
        /// </summary>
        protected Exception CaughtException { get; set; }

        /// <summary>
        /// The series of events that are pubished as a result of a state change within the aggregate.
        /// </summary>
        protected ThePublishedEvents PublishedEvents { get; private set; }


        protected BaseAggregateTestFixture()
        {
            InitializeFixture();
            ExecuteContext();
        }

        ~BaseAggregateTestFixture()
        {
            if (_container != null)
                _container.Dispose();
        }

        /// <summary>
        /// This will set up the initial testing context 
        /// for observing behavior by the aggregate.
        /// </summary>
        public virtual IEnumerable<Action>  Given()
        {
            return new List<Action>();
        }

        /// <summary>
        /// This will intialize the context for changing the domain 
        /// aggregate via an action (method) on the aggregate.
        /// </summary>
        public abstract void When();

        /// <summary>
        /// This will conduct any test condition
        /// tests or data checks.
        /// </summary>
        public virtual void Finally()
        {
        }

        /// <summary>
        /// Registers a compoent for interaction with tthe aggregate root.
        /// </summary>
        /// <typeparam name="TComponent">The concrete component to register</typeparam>
        protected void RegisterCollaborator<TComponent>() where TComponent : class
        {
            _container.Register(Component.For<TComponent>()
                                    .ImplementedBy<TComponent>());
        }

        /// <summary>
        /// Registers a compoent for interaction with tthe aggregate root.
        /// </summary>
        /// <typeparam name="TContract">Interface of the concrete component</typeparam>
        /// <typeparam name="TService">The concrete component to register</typeparam>
        protected void RegisterCollaborator<TContract, TService>()
            where TService : class, TContract
        {
            _container.Register(Component.For<TContract>()
                                    .ImplementedBy<TService>());
        }

        private void ExecuteContext()
        {
            CaughtException = new NoExceptionWasCaughtButOneWasExpectedException();

            PrepareAggregate();

            foreach(var preCondition in Given())
                preCondition.Invoke();

            try
            {
                When();

                foreach (var change in Aggregate.GetChanges())
                    _container.Resolve<IEventStorage>().Save(change);

            }
            catch (Exception e)
            {
                if(e is NotImplementedException)
                    throw e;

                CaughtException = e;
            }
            finally
            {
                Finally();
            }

            PublishedEvents = new ThePublishedEvents(_container);
        }

        private void PrepareAggregate()
        {
            Aggregate = _container.Resolve<IDomainRepository>().Create<TAGGREGATE>();
        }

        private void InitializeFixture()
        {
            _container = new WindsorContainer();

            _container.Register(Component.For<TAGGREGATE>()
                                    .ImplementedBy<TAGGREGATE>());

            _container.Register(Component.For<IDomainRepository>()
                               .ImplementedBy<DomainRepository>());

            _container.Register(Component.For<IEventStorage>()
                        .ImplementedBy<InMemoryEventStorage>()
                        .LifeStyle.Singleton);

        }
    }
}