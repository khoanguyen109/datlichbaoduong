using System;
using System.Web;
using NHibernate;
using System.Linq;
using System.Text;
using System.Data;
using NHibernate.Cfg;
using System.Reflection;
using NHibernate.Driver;
using NHibernate.Context;
using NHibernate.Dialect;
using System.Threading.Tasks;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Mapping.ByCode;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;

namespace MaintenanceSchedule.Library.NHibernate
{
    public class SessionFactory
    {
        private static SessionFactory instance = null;
        internal const string DefaultFactoryKey = "nhibernate.current_factory_key";
        private static Dictionary<string, ISessionFactory> _sessionFactories { get; set; }

        private SessionFactory()
        {
            _sessionFactories = new Dictionary<string, ISessionFactory>();
        }

        public static SessionFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new SessionFactory();
                return instance;
            }
        }

        public ISessionFactory GetSessionFactory()
        {
            return GetSessionFactory(DefaultFactoryKey);
        }

        public ISessionFactory GetSessionFactory(string factoryKey)
        {
            return _sessionFactories[factoryKey];
        }

        public ISession OpenSession()
        {
            return OpenSession(DefaultFactoryKey);
        }

        public ISession OpenSession(string factoryKey)
        {
            return _sessionFactories[factoryKey].OpenSession();
        }

        public void CloseSessions()
        {
            CloseSessions(DefaultFactoryKey);
        }

        public void CloseSessions(string factoryKey)
        {
            _sessionFactories[factoryKey].Close();
        }

        public void Initialize(string connectionString, Assembly mappingAssembly)
        {
            Initialize(DefaultFactoryKey, connectionString, mappingAssembly);
        }

        public void Initialize(string factoryKey, string connectionString, Assembly mappingAssembly)
        {
            var configuration = ConfigureForCurrentSessionContext(connectionString);
            var mapping = GetMappings(mappingAssembly);
            configuration.AddMapping(mapping);
            SchemaMetadataUpdater.QuoteTableAndColumns(configuration);
            var sessionFactory = configuration.BuildSessionFactory();

            if (sessionFactory == null)
                throw new InvalidOperationException("Session factory could not be built.");

            _sessionFactories.Add(factoryKey, sessionFactory);
        }

        private Configuration Configure(string connectionString)
        {
            var configure = new Configuration();
            configure.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2008Dialect>();
                db.Driver<SqlClientDriver>();
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = IsolationLevel.ReadCommitted;
                db.ConnectionStringName = connectionString;
                db.Timeout = 10;

                // enabled for testing
                db.LogFormattedSql = true;
                db.LogSqlInConsole = true;
                db.AutoCommentSql = true;
            });
            return configure;
        }

        private Configuration ConfigureForCurrentSessionContext(string connectionString)
        {
            var configure = HttpContext.Current != null
                ? GetConfigCurrentContext<WebSessionContext>(connectionString)
                : GetConfigCurrentContext<ThreadLocalSessionContext>(connectionString);

            return configure;
        }

        private Configuration GetConfigCurrentContext<T>(string connectionString) where T : ICurrentSessionContext
        {
            var configure = new Configuration();
            //configure.SessionFactoryName("NhibernateDemo");
            configure.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2012Dialect>();
                db.Driver<SqlClientDriver>();
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = IsolationLevel.ReadCommitted;
                db.ConnectionStringName = connectionString;
                db.Timeout = 10;

                // enabled for testing
                db.LogFormattedSql = true;
                db.LogSqlInConsole = true;
                db.AutoCommentSql = true;
            }).CurrentSessionContext<T>();

            return configure;
        }

        private HbmMapping GetMappings(Assembly mappingAssembly)
        {
            var mapper = new ModelMapper();
            mapper.AddMappings(mappingAssembly.GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            return mapping;
        }
    }
}
