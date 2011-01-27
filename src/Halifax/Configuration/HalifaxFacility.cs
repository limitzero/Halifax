using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Halifax.Bus.Commanding;
using Halifax.Bus.Eventing.Async.Endpoints;
using Halifax.Bus.Eventing.Async.Pipeline;
using Halifax.Bus.Eventing.Async.Subscriptions;
using Halifax.Bus.Eventing.Async.Transport;
using Halifax.Bus.Eventing.Async.Transport.Msmq;
using Halifax.Commanding;
using Halifax.Configuration.Bootstrapper;
using Halifax.Configuration.Builders;
using Halifax.Configuration.Elements;
using Halifax.Eventing;
using Halifax.Internals.Dispatchers;
using Halifax.Internals.Reflection;
using Halifax.Internals.Serialization;
using Halifax.Storage.Aggregates;
using Halifax.Storage.Events;

namespace Halifax.Configuration
{
    /// <summary>
    /// Facility to configure the application
    ///  Ex:
    ///  var container = new WindsorContainer(@"halifax.config.xml");
    ///  container.Kernel.AddFacility(HalifaxFacility.FACILITY_ID, new HalifaxFacility());
    /// </summary>
    public class HalifaxFacility : AbstractFacility
    {
        public const string FACILITY_ID = "halifax";
        private object[] _elementBuilders = {};
        private string _workingDirectory = string.Empty;

        protected override void Init()
        {
            #region -- register all of the default components --

            Kernel.AddComponent(typeof (IEndpointManager).Name,
                                typeof (IEndpointManager),
                                typeof (EndpointManager),
                                LifestyleType.Singleton);

            Kernel.AddComponent(typeof (ISubscriptionManager).Name,
                                typeof (ISubscriptionManager),
                                typeof (SubscriptionManager),
                                LifestyleType.Singleton);

            Kernel.AddComponent(typeof (IPipelineManager).Name,
                                typeof (IPipelineManager),
                                typeof (PipelineManager),
                                LifestyleType.Transient);

            Kernel.AddComponent(typeof (ISerializationProvider).Name,
                                typeof (ISerializationProvider),
                                typeof (DataContractSerializationProvider),
                                LifestyleType.Singleton);

            Kernel.Register(Component.For<EventEndpointBuilder>()
                                .ImplementedBy<EventEndpointBuilder>());

            Kernel.Register(Component.For<IReflection>()
                                .ImplementedBy<DefaultReflection>());

            Kernel.Register(Component.For<IUnitOfWork>()
                                .ImplementedBy<UnitOfWork>());

            Kernel.Register(Component.For<IStartableCommandBus>()
                                .ImplementedBy<InProcessCommandBus>());

            Kernel.Register(Component.For<ICommandMessageDispatcher>()
                                .ImplementedBy<CommandMessageDispatcher>());

            Kernel.Register(Component.For<IEventMessageDispatcher>()
                                .ImplementedBy<EventMessageDispatcher>());

            Kernel.Register(Component.For<ITransport>()
                                .ImplementedBy<MsmqTransport>());

            #endregion

            // find all of the element builders for parsing the configuration file:
            _elementBuilders = Kernel.Resolve<IReflection>()
                .FindConcreteTypesImplementingInterfaceAndBuild(typeof (AbstractElementBuilder),
                                                                GetType().Assembly);

            // configure the infrastructure:
            // BuildInfrastructureVia<ConfigurationElementBuilder>();
            BuildInfrastructureVia<EventBusElementBuilder>();
            BuildInfrastructureVia<MessagesElementBuilder>();
            BuildInfrastructureVia<EndpointElementBuilder>();
            //BuildInfrastructureVia<DomainElementBuilder>();
            //BuildInfrastructureVia<CommandHandlerElementBuilder>();
            //BuildInfrastructureVia<EventHandlerElementBuilder>();

            // configure all of the custom components:
            RunCustomBootStrappers();

            #region -- dependency checks for swapable components --

            try
            {
                var storage = Kernel.Resolve<IEventStorage>();
            }
            catch (Exception e)
            {
                // set the default storage for the domain events (in-memory)
                Kernel.Register(Component.For<IEventStorage>()
                                    .ImplementedBy<InMemoryEventStorage>()
                                    .LifeStyle.Singleton);
            }

            try
            {
                var repository = Kernel.Resolve<IDomainRepository>();
            }
            catch (Exception e)
            {
                // set the default lifecycle manager for the domain aggregates (in-memory)
                Kernel.Register(Component.For<IDomainRepository>()
                                    .ImplementedBy<DomainRepository>()
                                    .LifeStyle.Transient);
            }

            #endregion

            RegisterHandlers();
        }

        /// <summary>
        /// This will register all of the command and event consumers in the executable directory
        /// into the container for resolution at runtime.
        /// </summary>
        private void RegisterHandlers()
        {
            string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");

            foreach (var file in files)
            {
                try
                {
                    Assembly asm = Assembly.LoadFile(file);

                    Kernel.Register(AllTypes.FromAssembly(asm)
                                    .BasedOn(typeof(CommandConsumer.For<>))
                                    .WithService.Base());

                    Kernel.Register(AllTypes.FromAssembly(asm)
                                  .BasedOn(typeof(EventConsumer.For<>))
                                  .WithService.Base());
                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }

        private void RunCustomBootStrappers()
        {
            string[] files = {};

            if (!string.IsNullOrEmpty(_workingDirectory))
                files = Directory.GetFiles(_workingDirectory, "*.dll");
            else
            {
                files = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
            }

            foreach (string file in files)
            {
                try
                {
                    Assembly asm = Assembly.LoadFile(file);

                    object[] items = Kernel.Resolve<IReflection>()
                        .FindConcreteTypesImplementingInterfaceAndBuild(typeof (AbstractBootstrapper), asm);

                    if (items.Count() == 0) continue;

                    foreach (object item in items)
                    {
                        var bootstrapper = item as AbstractBootstrapper;
                        bootstrapper.WorkingDirectory = _workingDirectory;
                        ExecuteBootstrapper(bootstrapper);
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        private void BuildInfrastructureVia<TElementBuilder>()
            where TElementBuilder : AbstractElementBuilder
        {
            var builder = (from b in _elementBuilders
                           where b.GetType() == typeof (TElementBuilder)
                           select b).FirstOrDefault() as AbstractElementBuilder;

            for (int index = 0; index < FacilityConfig.Children.Count; index++)
            {
                IConfiguration element = FacilityConfig.Children[index];

                if (element == null)
                    continue;

                if (builder.IsMatchFor(element.Name))
                {
                    if (!string.IsNullOrEmpty(_workingDirectory))
                        builder.WorkingDirectory = _workingDirectory;

                    builder.Kernel = Kernel;
                    builder.Build(element);

                    if (builder is ConfigurationElementBuilder)
                        _workingDirectory = (builder as ConfigurationElementBuilder).WorkingDirectory;

                    break;
                }
            }
        }

        private void ExecuteBootstrapper(AbstractBootstrapper bootstrapper)
        {
            try
            {
                if (!bootstrapper.IsActive) return;

                bootstrapper.Kernel = Kernel;
                bootstrapper.Configure();
            }
            catch (Exception e)
            {
            }
        }
    }
}