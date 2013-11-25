using System;
using System.Linq;

namespace Halifax.Read
{
	/// <summary>
	/// Marker interface to denote the result of a query.
	/// </summary>
	/// <typeparam name="TResult">Type of result returned from a query</typeparam>
	public interface IQueryResult<TResult> : IQueryOn
	{
		/// <summary>
		/// Gets or sets the result of the query being executed.
		/// </summary>
		TResult Result { get; set; }
	}

	/// <summary>
	/// Marker interface to denote an object that can execute a query.
	/// </summary>
	/// <typeparam name="TReadModel">The model class to execute the query over.</typeparam>
	public interface IQueryExecuter<TReadModel> where TReadModel : class , IReadModel
	{
		/// <summary>
		/// This will execute the query per specification over the collection of models
		/// per the desired criteria.
		/// </summary>
		/// <param name="queryable"></param>
		 void Execute(IQueryable<TReadModel> queryable);
	}

	/// <summary>
	/// The query class provides a way to execute a query  over the given 
	/// read model per custom critieria.
	/// </summary>
	/// <typeparam name="TReadModel">Read model to query over</typeparam>
	/// <typeparam name="TResult">The result of the query when executed.</typeparam>
	[Serializable]
	public abstract class Query<TReadModel, TResult> : 
		IQueryResult<TResult>,
		IQueryExecuter<TReadModel> where TReadModel : class, IReadModel
	{
		public TResult Result { get; set; }
		public abstract void Execute(IQueryable<TReadModel> queryable);
	}
}