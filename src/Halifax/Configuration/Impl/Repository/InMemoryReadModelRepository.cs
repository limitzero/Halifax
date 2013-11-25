using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Halifax.Read;

namespace Halifax.Configuration.Impl.Repository
{
	public class InMemoryReadModelRepository<TReadModel>
		: IReadModelRepository<TReadModel> where TReadModel : class, IReadModel
	{
		private static ConcurrentBag<IReadModel> storage;

		public InMemoryReadModelRepository()
		{
			if (storage == null)
			{
				storage = new ConcurrentBag<IReadModel>();
			}
		}

		~InMemoryReadModelRepository()
		{
			storage = null;
		}

		public TReadModel Get(Guid id)
		{
			return storage.FirstOrDefault(model => model.Id.Equals(id)) as TReadModel;
		}

		public void Insert(TReadModel model)
		{
			storage.Add(model);
		}

		public void Update(TReadModel model)
		{
			var old_model = storage.FirstOrDefault(m => m.Id.Equals(model.Id));

			if (old_model == null)
			{
				Insert(model);
			}
			else
			{
				storage.TryTake(out old_model);
				Insert(model);
			}
		}

		public void Delete(TReadModel model)
		{
			IReadModel result = model;
			storage.TryTake(out result);
		}

		public IEnumerable<TReadModel> All()
		{
			var models = storage.Where(model => typeof (TReadModel) == model.GetType())
				.Select(model => model as TReadModel).ToList();
			return models;
		}

		public void Query(IQueryExecuter<TReadModel> query)
		{
			query.Execute(this.All().AsQueryable());
		}
	}
}