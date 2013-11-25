using System;
using System.Collections.Generic;

namespace Halifax.Configuration
{
	/// <summary>
	/// Interface for interaction wih different DI containers
	/// </summary>
	public interface IContainer : IDisposable
	{
		IEnumerable<TComponent> ResolveAll<TComponent>();
		IEnumerable<object> ResolveAll(Type component);
		TComponent Resolve<TComponent>();
		object Resolve(Type component);
		void Register<TContract, TService>() where TService : class, TContract;
		void Register<TComponent>() where TComponent : class;
		void Register(Type contract, Type service);
		void Register(Type service);
		void RegisterInstance(Type contract, object service);
		void RegisterInstance<TContract, TService>(TService service) where TService : class, TContract;
		void RegisterInstance<TContract>(object service);
		void RegisterFactory<TContract>(Func<IContainer, TContract> factory);
	}
}