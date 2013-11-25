using System;
using NHibernate;

namespace Halifax.NHibernate
{
	public interface INHibernateSchemaManager 
	{
		ISessionFactory GetSessionFactory();
		void CreateSchema();
		void DropSchema();
	}
}