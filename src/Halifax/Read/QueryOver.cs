using System;

namespace Halifax.Read
{
	/// <summary>
	/// The query executer provides a way to execute query classes over the given 
	/// read model per custom critieria.
	/// </summary>
	/// <typeparam name="TReadModel">Read model to query over</typeparam>
	[Obsolete]
	public class QueryOver<TReadModel> where TReadModel : class, IReadModel
	{
		private readonly IReadModelRepository<TReadModel> repository;

		public QueryOver(IReadModelRepository<TReadModel> repository)
		{
			this.repository = repository;
		}

		public TResult With<TResult>(Query<TReadModel, TResult> query) 
			where TResult : class
		{
			repository.Query(query);
			return query.Result;
		}
	}
}