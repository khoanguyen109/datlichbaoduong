using FluentNHibernate.Cfg;
using NHibernate;
using NHib = NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Context;
using yellowx.Framework.Extensions;

namespace yellowx.Framework.Data.NHibernate
{
    public static class NHibernateSessionFactory
    {
        /// <summary>
        /// The default factory key used if only one database is being communicated with.
        /// </summary>
        internal const string DefaultFactoryKey = "nhibernate.current_factory_key";
        private static readonly Dictionary<string, ISessionFactory> sessionFactories;
        private static readonly Dictionary<string, IInterceptor> interceptors;//hook into session crud. not use now.

        static NHibernateSessionFactory()
        {
            sessionFactories = new Dictionary<string, ISessionFactory>();
            interceptors = new Dictionary<string, IInterceptor>();
        }
        public static NHib.Cfg.Configuration Initialize(string[] mappingAssemblies, string cfgFile)
        {
            return Initialize(mappingAssemblies, cfgFile, false);
        }
        public static NHib.Cfg.Configuration Initialize(string[] mappingAssemblies, string cfgFile, bool createSchema = false)
        {
            return Initialize(DefaultFactoryKey, mappingAssemblies, cfgFile, null, createSchema);
        }

        public static NHib.Cfg.Configuration Initialize(string factoryKey, string[] mappingAssemblies, string cfgFile)
        {
            return Initialize(factoryKey, mappingAssemblies, cfgFile, null, false);
        }
        public static NHib.Cfg.Configuration Initialize(string factoryKey, string[] mappingAssemblies, string cfgFile, IDictionary<string, string> cfgProperties, bool createSchema = false)
        {
            var cfg = ConfigureNHibernate(cfgFile, cfgProperties);
            var sessionFactory = CreateSessionFactory(mappingAssemblies, cfg, createSchema);
            sessionFactories.Add(factoryKey.IsNullOrEmpty() ? DefaultFactoryKey : factoryKey, sessionFactory);
            return cfg;
        }

        #region Implement INHibernateSessionFactory interface

        public static ISession GetCurrentSession()
        {
            return GetCurrentSession(DefaultFactoryKey);
        }
        public static ISession GetCurrentSession(string factoryKey)
        {
            var sessionFactory = GetSessionFactory(factoryKey);
            if (CurrentSessionContext.HasBind(sessionFactory))
                return sessionFactory.GetCurrentSession();
            var session = interceptors.ContainsKey(factoryKey) ? sessionFactory.OpenSession(interceptors[factoryKey]) : sessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
            return session;
        }
        public static void CloseSessions()
        {
            foreach (var sf in sessionFactories)
            {
                if (CurrentSessionContext.HasBind(sf.Value))
                {
                    var session = CurrentSessionContext.Unbind(sf.Value);
                    session.Close();
                }
            }
        }
        #endregion

        #region Private methods
        private static ISessionFactory GetSessionFactory()
        {
            return GetSessionFactory(DefaultFactoryKey);
        }
        private static ISessionFactory GetSessionFactory(string factoryKey)
        {
            return sessionFactories[factoryKey];
        }
        private static string MakeLoadReadyAssemblyName(string assemblyName)
        {
            return (assemblyName.IndexOf(".dll") == -1)
                ? assemblyName.Trim() + ".dll"
                : assemblyName.Trim();
        }
        private static NHib.Cfg.Configuration ConfigureNHibernate(string configFile, IDictionary<string, string> cfgProperties)
        {
            var cfg = new NHib.Cfg.Configuration();

            if (cfgProperties != null)
                cfg.AddProperties(cfgProperties);

            if (string.IsNullOrEmpty(configFile))
                return cfg.Configure();

            return cfg.Configure(configFile);
        }

        private static ISessionFactory CreateSessionFactory(string[] mappingAssemblies, NHib.Cfg.Configuration cfg, bool createSchema = false)
        {
            var fluentConfiguration = Fluently.Configure(cfg);

            fluentConfiguration.Mappings(m =>
            {
                foreach (var mappingAssembly in mappingAssemblies)
                {
                    var assembly = Assembly.LoadFrom(MakeLoadReadyAssemblyName(mappingAssembly));
                    m.HbmMappings.AddFromAssembly(assembly);
                    m.FluentMappings.AddFromAssembly(assembly);
                }
            });

            if (createSchema)
                fluentConfiguration.ExposeConfiguration(config => new SchemaUpdate(config).Execute(false, true));
            return fluentConfiguration.BuildSessionFactory();
        }
        #endregion
    }

}
