using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel;
using Halifax.Bus.Eventing.Async.Endpoints;
using Halifax.Bus.Eventing.Async.Subscriptions;
using Halifax.Eventing;

namespace Halifax.Configuration.Builders
{
    /// <summary>
    /// Builder for associating a set of messages with a particular transport and 
    /// physical location for storage.
    /// 
    /// Ex:
    /// builder.Build("localhost@myQueue",
    ///                     Messages.Configure.FromAssemblyContaining<CreateProductCommand>(),
    ///                      Scheduling.Configure.WithMulitpleConsumers(1, 1));
    /// </summary>
    public class EventEndpointBuilder
    {
        private readonly IKernel _kernel;

        public EventEndpointBuilder(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Build(string location,
                          MessageConfiguration messageConfiguration,
                          SchedulingOptions schedulingOptions)
        {
            // build the subscriptions based on the messages:
            Type[] possibleMessages = (from message in messageConfiguration.MessageAssembly.GetTypes()
                                       where message.IsClass
                                             && message.IsAbstract == false
                                       select message).ToArray();

            // only publishing events in a distributed manner:
            Type[] publishableMessages = (from msg in possibleMessages
                                          where typeof (DomainEvent).IsAssignableFrom(msg)
                                          select msg).ToArray();

            Type[] messages = publishableMessages;

            if (!string.IsNullOrEmpty(messageConfiguration.ConstrainingNamespace))
            {
                Type[] constrainedMessages = (from message in publishableMessages
                                              where
                                                  message.FullName.StartsWith(messageConfiguration.ConstrainingNamespace)
                                              select message).ToArray();

                if (constrainedMessages.Length > 0)
                    messages = constrainedMessages;
            }

            var subscriptionManager = _kernel.Resolve<ISubscriptionManager>();
            foreach (Type message in messages)
                subscriptionManager.RegisterSubscription(location, message);

            // build the transports and polling intervals:
            var endpointManager = _kernel.Resolve<IEndpointManager>();
            endpointManager.RegisterEndpoint(location);
        }
    }

    public class Messages
    {
        private static readonly MessageConfiguration _configuration = new MessageConfiguration();

        public static MessageConfiguration Configure
        {
            get { return _configuration; }
        }
    }

    public class MessageConfiguration
    {
        public Assembly MessageAssembly { get; private set; }
        public string ConstrainingNamespace { get; private set; }

        public virtual MessageConfiguration FromAssemblyContaining<TType>()
        {
            return FromAssemblyContaining(typeof (TType));
        }

        public virtual MessageConfiguration FromAssemblyContaining(Type containingType)
        {
            MessageAssembly = containingType.Assembly;
            return this;
        }

        public virtual MessageConfiguration FromAssemblyFile(string assemblyFile)
        {
            MessageAssembly = Assembly.LoadFile(assemblyFile);
            return this;
        }

        public virtual MessageConfiguration FromAssembly(string assemblyName)
        {           
            MessageAssembly = Assembly.Load(assemblyName);
            return this;
        }

        public virtual MessageConfiguration StartingWith(string constrainingNamespace)
        {
            ConstrainingNamespace = constrainingNamespace;
            return this;
        }
    }

    public class Scheduling
    {
        private static readonly SchedulingOptions _configuration = new SchedulingOptions();

        public static SchedulingOptions Configure
        {
            get { return _configuration; }
        }
    }

    public class SchedulingOptions
    {
        public SchedulingOptions()
        {
            Consumers = new CompetingConsumersOption();
            Polled = new PeriodicPollingOption();
        }

        public CompetingConsumersOption Consumers { get; set; }
        public PeriodicPollingOption Polled { get; set; }

        public SchedulingOptions WithMulitpleConsumers(int numberOfThreads)
        {
            Consumers.HavingConcurrencyOf(numberOfThreads).HavingWaitIntervalOf(1);
            return this;
        }

        public SchedulingOptions WithMulitpleConsumers(int numberOfThreads, int waitInterval)
        {
            Consumers.HavingConcurrencyOf(numberOfThreads).HavingWaitIntervalOf(waitInterval);
            return this;
        }

        public SchedulingOptions WithPollingEvery(int numberOfSeconds)
        {
            Polled.PolledEvery(numberOfSeconds);
            return this;
        }
    }

    public class CompetingConsumersOption
    {
        public int Concurrency { get; private set; }
        public int WaitInterval { get; private set; }


        public CompetingConsumersOption HavingConcurrencyOf(int numberOfThreads)
        {
            Concurrency = numberOfThreads;
            return this;
        }


        public CompetingConsumersOption HavingWaitIntervalOf(int timeToWaitInSeconds)
        {
            WaitInterval = timeToWaitInSeconds;
            return this;
        }
    }

    public class PeriodicPollingOption
    {
        public int Polled { get; private set; }

        public PeriodicPollingOption PolledEvery(int numberOfSeconds)
        {
            Polled = numberOfSeconds;
            return this;
        }
    }
}