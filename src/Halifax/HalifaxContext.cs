using System;
using Halifax.Commands;
using Halifax.Configuration;
using Halifax.Internals.Commands;
using Halifax.Read;

namespace Halifax
{
	//TODO: This is not the nicest way to accesss the infrastructure (singleton), think a little more on this one

	/// <summary>
	/// Stand-alone context source to wire-up the infrastructure for Hailfax and 
	/// provide convience methods for interacting with the underlying infrastructure
	/// </summary>
	public class HalifaxContext
	{
		private static HalifaxContext instance;
		private Configuration.Impl.Configuration configuration;

		private HalifaxContext()
		{
			this.configuration = new Configuration.Impl.Configuration();
		}

		~HalifaxContext()
		{
			instance = null;
			if(configuration != null)
			{
				configuration.Dispose();
			}
			configuration = null;
		}

		/// <summary>
		/// Singleton instance to access the underlying configuration 
		/// to set the options for this instance of the executing context 
		/// on infrastructure concerns.
		/// </summary>
		public static HalifaxContext ConfigurationFactory
		{
			get { return new Factory().Create(); }
		}

		/// <summary>
		/// Configures the infrastructure with your desired elements.
		/// </summary>
		/// <returns></returns>
		public IConfiguration ConfigureWith()
		{
			return this.configuration;
		}

		/// <summary>
		/// This will execute a query defined by the specific query object and return the results to the caller.
		/// </summary>
		/// <typeparam name="TReadModel"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="query">The current query</param>
		/// <returns></returns>
		public TResult QueryBy<TReadModel, TResult>(Query<TReadModel, TResult> query)
			where TReadModel : class, IReadModel
			where TResult : class
		{
			TResult result = default(TResult);

			var query_over_type = typeof (QueryOver<>).MakeGenericType(new Type[] {typeof (TReadModel)});
			var query_over_instance = this.configuration.CurrentContainer().Resolve(query_over_type);
			
			if(query_over_instance != null)
			{
				result = ((QueryOver<TReadModel>) query_over_instance).With(query);
			}

			return result;
		}

		/// <summary>
		/// This will send a command to the aggregate in the domain model for behavioral state changes 
		/// that will result in the changing of the associated information to the persistance models via 
		/// events. 
		/// </summary>
		/// <typeparam name="TCommand">Type of command to send</typeparam>
		/// <param name="command">The current command</param>
		/// <returns></returns>
		public CommandResult SendCommand<TCommand>(TCommand command) where  TCommand : Command
		{
			var command_bus = this.configuration.CurrentContainer().Resolve<ICommandBus>();
			return command_bus.Send(command);
		}

		private class Factory
		{
			public HalifaxContext Create()
			{
				if(instance == null)
				{
					instance = new HalifaxContext();
				}

				return instance;
			}
		}
	}
}