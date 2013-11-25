using System;
using System.Collections.Generic;

namespace Halifax.Read
{
	public interface IReadModelRepository
	{
		
	}

	/// <summary>
	/// This is the core interface that provides basic CRUD operations 
	/// for entities projected from the aggregate root (or the "behavioral model")
	/// </summary>
	/// <typeparam name="TReadModel"></typeparam>
	public interface IReadModelRepository<TReadModel> : IReadModelRepository where TReadModel : class , IReadModel
	{
		TReadModel Get(Guid id);
		void Insert(TReadModel model);
		void Update(TReadModel model);
		void Delete(TReadModel model);
		IEnumerable<TReadModel> All();
		void Query(IQueryExecuter<TReadModel> query);
	}
}