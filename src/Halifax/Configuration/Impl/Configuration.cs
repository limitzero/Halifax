using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Halifax.Commands;
using Halifax.Configuration.Impl.Containers;
using Halifax.Configuration.Impl.Eventing;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Configuration.Impl.Repository;
using Halifax.Configuration.Impl.Serialization;
using Halifax.Domain;
using Halifax.Domain.Internal;
using Halifax.Events;
using Halifax.Internals;
using Halifax.Internals.Commands;
using Halifax.Internals.Dispatchers;
using Halifax.Internals.Dispatchers.Impl;
using Halifax.Internals.Reflection;
using Halifax.Read;
using Halifax.StateMachine;
using Halifax.StateMachine.Impl;

namespace Halifax.Configuration.Impl
{
	public class Configuration : IConfiguration
	{
		private bool disposed;
		private IContainer current_container;
		
		public void BindContainer(IContainer container)
		{
			this.current_container = container;
		}

		public IContainer CurrentContainer()
		{
			return this.current_container;
		}

		/// <summary>
		/// Configures the options for component container using for resolving 
		/// dependencies of code at runtime.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public IConfiguration Container(Func<ContainerOptions, ContainerOptions> options)
		{
			options(new ContainerOptions(this));
			return this;
		}

		/// <summary>
		/// Configures the options for publication of events to the consumer(s) of the events.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public IConfiguration Eventing(Func<EventingOptions, EventingOptions> options)
		{
			options(new EventingOptions(this.current_container));
			return this;
		}

		/// <summary>
		/// Configures the options for serialization of messages to persistance storage mechanism 
		/// for the event store.
		/// </summary>
		/// <param name="options">Storage configuration options.</param>
		/// <returns></returns>
		public IConfiguration Serialization(Func<SerializationOptions, SerializationOptions> options)
		{
			options(new SerializationOptions(this.current_container));
			return this;
		}

		/// <summary>
		/// Configures the options for event storage.
		/// </summary>
		/// <param name="options">Storage configuration options.</param>
		/// <returns></returns>
		public IConfiguration EventStore(Func<EventStorageOptions, EventStorageOptions> options)
		{
			options(new EventStorageOptions(this.current_container));
			return this;
		}

		/// <summary>
		/// Configures the options for read model (i.e. repository) as persistance storage and retrieval of entities.
		/// </summary>
		/// <param name="options">Repository configuration options.</param>
		/// <returns></returns>
		public IConfiguration ReadModel(Func<ReadModelOptions, ReadModelOptions> options)
		{
			options(new ReadModelOptions(this.current_container));
			return this;
		}

		/// <summary>
		/// This will be the last option chosen and will configure the infrastructure based on 
		/// the options selected.
		/// </summary>
		/// <param name="includedAssemblies">Any extra assemblies that should be included for participation</param>
		public void Configure(params Assembly[] includedAssemblies)
		{
			if(this.disposed == true) return;

			this.CurrentContainer().Register<ICommandBus, InProcessCommandBus>();
			var assemblies = this.LoadAllReferencedAssemblies(includedAssemblies);

			RegisterAggregateRoots(assemblies);
			RegisterReadModels(assemblies);
			RegisterQueryModels(assemblies);
			RegisterCommands(assemblies);
			RegisterEvents(assemblies);
			RegisterCommandValidators(assemblies);
			RegisterCommandHandlers(assemblies);
			RegisterEventHandlers(assemblies);
			RegisterInfrastructure();
			RegisterExternalDependenciesInContainer(assemblies);
			InitializeSerializer();

			// register the container in the underlying instance for self-resolution:
			this.CurrentContainer().RegisterInstance<IContainer>(this.current_container);
		}

		public void Dispose()
		{
			Disposing(true);
			GC.SuppressFinalize(this);
		}

		private void Disposing(bool disposing)
		{
			if (disposing == true)
			{
				if (this.current_container != null)
				{
					this.current_container.Dispose();
				}
				this.current_container = null;
			}

			this.disposed = true;
		}

		private void RegisterAggregateRoots(IEnumerable<Assembly> assemblies)
		{
			var aggregate_roots = (from asm in assemblies
								   from type in asm.GetTypes()
								   where typeof(AggregateRoot).IsAssignableFrom(type) &&
										 type.IsClass &&
										 type.IsAbstract == false
								   select type).Distinct().ToList();

			aggregate_roots.ForEach(ar => this.CurrentContainer().Register(ar));
		}

		private void RegisterReadModels(IEnumerable<Assembly> assemblies)
		{
			var models = (from asm in assemblies
						  from type in asm.GetTypes()
						  where typeof(IReadModel).IsAssignableFrom(type) &&
								type.IsClass &&
								type.IsAbstract == false
						  select type).Distinct().ToList();

			models.ForEach(model => this.CurrentContainer().Register(model));
		}

		private void RegisterQueryModels(IEnumerable<Assembly> assemblies)
		{
			var models = (from asm in assemblies
						  from type in asm.GetTypes()
						  where typeof(IQueryOn).IsAssignableFrom(type) &&
								type.IsClass &&
								type.IsAbstract == false
						  select type).Distinct().ToList();

			models.ForEach(model => this.CurrentContainer().Register(model));
		}

		private void RegisterCommands(IEnumerable<Assembly> assemblies)
		{
			var commands = (from asm in assemblies
							from type in asm.GetTypes()
							where typeof(Command).IsAssignableFrom(type) &&
								  type.IsClass &&
								  type.IsAbstract == false
							select type).Distinct().ToList();

			commands.ForEach(c => this.CurrentContainer().Register(c));
		}

		private void RegisterEvents(IEnumerable<Assembly> assemblies)
		{
			var events = (from asm in assemblies
						  from type in asm.GetTypes()
						  where typeof(Event).IsAssignableFrom(type) &&
								type.IsClass &&
								type.IsAbstract == false
						  select type).Distinct().ToList();

			events.ForEach(e => this.CurrentContainer().Register(e));
		}

		private void RegisterCommandValidators(IEnumerable<Assembly> assemblies)
		{
			var validators = (from asm in assemblies
							from type in asm.GetTypes()
							where typeof(ICommandValidatorFor).IsAssignableFrom(type)
							&& type.IsClass == true
							&& type.IsAbstract == false
							select type).Distinct().ToList();

			foreach (var validator in validators)
			{
				this.CurrentContainer().Register(validator);
			}
		}

		private void RegisterCommandHandlers(IEnumerable<Assembly> assemblies)
		{
			var handlers = (from asm in assemblies
							from type in asm.GetTypes()
							where typeof(CommandConsumer.For).IsAssignableFrom(type)
							select type).Distinct().ToList();

			foreach (var handler in handlers)
			{
				this.CurrentContainer().Register(handler);
			}
		}

		private void RegisterEventHandlers(IEnumerable<Assembly> assemblies)
		{
			var handlers = (from asm in assemblies
							from type in asm.GetTypes()
							where typeof(EventConsumer.For).IsAssignableFrom(type) == true
							&& type.IsClass == true
							&& type.IsAbstract == false
							select type).Distinct().ToList();

			foreach (var handler in handlers)
			{
				this.CurrentContainer().Register(handler);
			}
		}

		private void RegisterExternalDependenciesInContainer(IEnumerable<Assembly> assemblies)
		{
			var configurators = (from asm in assemblies
								 from type in asm.GetTypes()
								 where typeof(ICanConfigureContainer).IsAssignableFrom(type)
								 select type).Distinct().ToList();

			foreach (var configurator in configurators)
			{
				try
				{
					var configuration = Activator.CreateInstance(configurator) as ICanConfigureContainer;

					if (configuration != null)
					{
						configuration.Configure(this.CurrentContainer());
					}
				}
				catch (Exception exc)
				{
					var hex = new Halifax.Internals.Exceptions.HalifaxException(
						string.Format("An error occurred while attempting to configure the container for external dependencies using '{0}'. Reason: {1}",
						configurator.FullName, exc.Message), exc);
					throw hex;
				}
			}
		}

		private void RegisterInfrastructure()
		{
			IContainer container = this.CurrentContainer();
			container.Register<IReflection, DefaultReflection>();
			container.Register<IUnitOfWork, UnitOfWork>();
			container.Register<ICommandMessageDispatcher, CommandMessageDispatcher>();
			container.Register<IEventMessageDispatcher, EventMessageDispatcher>();
			container.Register<IAggregateRootRepository, AggregateRootRepository>();
			container.Register(typeof(QueryOver<>));
			container.Register<StateMachineExecuter>();
			container.Register<IBus, DefaultBus>();
		}

		private void InitializeSerializer()
		{
			var types = new List<Type>();
			var commands = this.CurrentContainer().ResolveAll<Command>();
			var events = this.CurrentContainer().ResolveAll<Event>();

			types.AddRange(commands.Select(c => c.GetType()));
			types.AddRange(events.Select(e => e.GetType()));

			types.Add(typeof(Event));
			types.Add(typeof(Command));

			this.CurrentContainer().Resolve<ISerializationProvider>().Initialize(types);
		}

		private IEnumerable<Assembly> LoadAllReferencedAssemblies(IEnumerable<Assembly> includedAssemblies)
		{
			var assemblies_to_inspect = new List<AssemblyName>(includedAssemblies.Select(a => a.GetName()));
			assemblies_to_inspect.AddRange(GetType().Assembly.GetReferencedAssemblies());

			var assemblies = assemblies_to_inspect
				.Where(a => a != typeof(AggregateRoot).Assembly.GetName()) // excluded framework assembly
				.Where(a => a.FullName.StartsWith("System") == false) // exclude .NET 
				.Where(a => a.FullName.StartsWith("mscorlib") == false) // exclude .NET 
				.Select(a => Assembly.Load(a)).ToList();

			return assemblies;
		}
	}
}