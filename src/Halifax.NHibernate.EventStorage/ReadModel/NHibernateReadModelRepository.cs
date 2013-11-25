using System;
using System.Collections.Generic;
using System.Linq;
using Halifax.Read;

namespace Halifax.NHibernate.ReadModel
{
	public class NHibernateReadModelRepository<TReadModel> : 
		IReadModelRepository<TReadModel> where TReadModel : class, IReadModel
	{
		private readonly INHibernateReadModelSessionFactory session_factory;
		
		public NHibernateReadModelRepository(INHibernateReadModelSessionFactory sessionFactory)
		{
			session_factory = sessionFactory;
		}

		public TReadModel Get(Guid id)
		{
			TReadModel readModel = default(TReadModel);

			using(var session = session_factory.Factory.OpenSession())
			using (var txn = session.BeginTransaction())
			{
				try
				{
					readModel = session.Get<TReadModel>(id);
					txn.Commit();
				}
				catch
				{
					txn.Rollback();
					throw;
				}
			}

			return readModel;
		}

		public void Insert(TReadModel model)
		{
			using (var session = session_factory.Factory.OpenSession())
			using (var txn = session.BeginTransaction())
			{
				try
				{
					session.Save(model);
					txn.Commit();
				}
				catch
				{
					txn.Rollback();
					throw;
				}
			}
		}

		public void Update(TReadModel model)
		{
			using (var session = session_factory.Factory.OpenSession())
			using(var txn = session.BeginTransaction())
			{
				try
				{
					session.SaveOrUpdate(model);
					txn.Commit();
				}
				catch
				{
					txn.Rollback();
					throw;
				}
				
			}
		}

		public void Delete(TReadModel model)
		{
			using (var session = session_factory.Factory.OpenSession())
			using (var txn = session.BeginTransaction())
			{
				try
				{
					session.Delete(model);
					txn.Commit();
				}
				catch 
				{
					txn.Rollback();
					throw;
				}
			}
		}

		public IEnumerable<TReadModel> All()
		{
			using (var session = session_factory.Factory.OpenSession())
			{
				var criteria = session.CreateCriteria<TReadModel>();
				var results = criteria.List<TReadModel>();
				return results;
			}
		}

		public void Query(IQueryExecuter<TReadModel> query)
		{
			query.Execute(this.All().AsQueryable());
		}
	}
}