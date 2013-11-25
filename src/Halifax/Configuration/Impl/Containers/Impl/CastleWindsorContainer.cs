using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Facilities.FactorySupport;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace Halifax.Configuration.Impl.Containers.Impl
{
	public class CastleWindsorContainer : IContainer
	{
		private IKernel kernel;

		public CastleWindsorContainer()
		{
			this.kernel = new DefaultKernel();
		}

		public void Dispose()
		{
			if(this.kernel != null)
			{
				this.kernel.Dispose();
			}
			this.kernel = null;
		}

		public IEnumerable<TComponent> ResolveAll<TComponent>()
		{
			List<TComponent> resolved_components = new List<TComponent>();

			var components = this.kernel.ResolveAll(typeof (TComponent));

			if(components != null && components.Length > 0)
			{
				foreach (var component in components)
				{
					TComponent resolved_component = default(TComponent);

					try
					{
						resolved_component = (TComponent) component;
						resolved_components.Add(resolved_component);
					}
					catch
					{
						continue;
					}
				}
			}

			return resolved_components;
		}

		public IEnumerable<object> ResolveAll(Type component)
		{
			Array resolved_instances = this.kernel.ResolveAll(component);
			var results = resolved_instances.Cast<object>().ToList();
			return results;
		}

		public TComponent Resolve<TComponent>()
		{
			return this.kernel.Resolve<TComponent>();
		}

		public object Resolve(Type component)
		{
			return this.kernel.Resolve(component);
		}

		public void Register<TContract, TService>() where TService : class, TContract
		{
			this.Register(typeof(TContract), typeof(TService));
		}

		public void Register<TComponent>() where TComponent : class
		{
			this.Register(typeof(TComponent));
		}

		public void Register(Type contract, Type service)
		{
			this.kernel.Register(Component.For(contract)
			                     	.ImplementedBy(service)
									.Named(string.Format("{0}-{1}-{2}", 
									Guid.NewGuid().ToString(), 
									contract.Name, 
									service.Name)));
		}

		public void Register(Type service)
		{
			this.kernel.Register(Component.For(service)
					.Named(string.Format("{0}-{1}", 
									Guid.NewGuid().ToString(),  
									service.Name)));
		}

		public void RegisterInstance(Type contract, object service)
		{
			this.kernel.Register(Component.For(contract).Instance(service));
		}

		public void RegisterInstance<TContract, TService>(TService service) where TService : class, TContract
		{
			this.RegisterInstance(typeof(TContract), service);
		}

		public void RegisterInstance<TContract>(object service)
		{
			this.RegisterInstance(typeof(TContract), service);
		}

		public void RegisterFactory<TContract>(Func<IContainer, TContract> factory)
		{
			this.kernel.AddFacility("factory", new FactorySupportFacility());

			Func<TContract> create = () => factory(this);

			this.kernel.Register(Component.For<TContract>().
							   UsingFactoryMethod(() => create())
							   .LifeStyle.Is(LifestyleType.Transient));
		}
	}
}