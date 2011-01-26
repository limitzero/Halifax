using NHibernate;

namespace Halifax.NHibernate.EventStorage
{
    public class SchemaManager
    {
        private static global::NHibernate.Cfg.Configuration _configuration = null; 
        private static ISessionFactory _sessionFactory;

        static SchemaManager()
        {
            if(_configuration == null)
            {
                _configuration = new global::NHibernate.Cfg.Configuration();
                _configuration.Configure();
            }
        }

        public static ISessionFactory GetSessionFactory()
        {
            if(_configuration != null)
            {
                if(_sessionFactory == null)
                _sessionFactory = _configuration.BuildSessionFactory();
            }

            return _sessionFactory;
        }

        public static void CreateSchema()
        {
            var exporter = new global::NHibernate.Tool.hbm2ddl.SchemaExport(_configuration);
            exporter.Execute(true, true, false);
        }

        public static void DropSchema()
        {
            var exporter = new global::NHibernate.Tool.hbm2ddl.SchemaExport(_configuration);
            exporter.Execute(true, true, true);
        }

        ~SchemaManager()
        {
            if (_sessionFactory != null)
                _sessionFactory = null;

            if (_configuration != null)
                _configuration = null;
        }
    }
}