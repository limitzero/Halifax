using System;
using System.Reflection;
using Halifax.Configuration.Impl.Containers;
using Halifax.Configuration.Impl.Eventing;
using Halifax.Configuration.Impl.EventStorage;
using Halifax.Configuration.Impl.Repository;
using Halifax.Configuration.Impl.Serialization;

namespace Halifax.Configuration
{
	public interface IConfiguration : IDisposable
	{
		/// <summary>
		/// Configures the options for dependency resolution via a component container.
		/// </summary>
		/// <param name="options">Container options.</param>
		/// <returns></returns>
		IConfiguration Container(Func<ContainerOptions, ContainerOptions> options);

		/// <summary>
		/// Configures the serialization of messages for storage in the event store.
		/// </summary>
		/// <param name="options">Serialization options.</param>
		/// <returns></returns>
		IConfiguration Serialization(Func<SerializationOptions, SerializationOptions> options);

		/// <summary>
		/// Configures the eventing mode of messages to event consumers.
		/// </summary>
		/// <param name="options">Eventing options.</param>
		/// <returns></returns>
		IConfiguration Eventing(Func<EventingOptions, EventingOptions> options);

		/// <summary>
		/// Configures the storage of events emitted from aggregate roots for storage.
		/// </summary>
		/// <param name="options">Event storage options.</param>
		/// <returns></returns>
		IConfiguration EventStore(Func<EventStorageOptions, EventStorageOptions> options);

		/// <summary>
		/// Configures the options for read model (i.e. repository) as persistance storage and retrieval of entities.
		/// </summary>
		/// <param name="options">Repository configuration options.</param>
		/// <returns></returns>
		IConfiguration ReadModel(Func<ReadModelOptions, ReadModelOptions> options);

		/// <summary>
		/// Called by the configuration of the container to bind it to the configuration.
		/// </summary>
		/// <param name="container"></param>
		void BindContainer(IContainer container);

		/// <summary>
		/// This will retrieve the current instance of the configured component container for 
		/// dependency resolution.
		/// </summary>
		/// <returns></returns>
		IContainer CurrentContainer();

		/// <summary>
		/// This will configure the infrastructure with the options selected using the indicated assemblies as the 
		/// participants in the infrastructure (i.e. command handlers, event handlers, etc.)
		/// </summary>
		/// <param name="includedAssemblies"></param>
		void Configure(params Assembly[] includedAssemblies);

		
	}
}