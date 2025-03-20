using System.Collections.Generic;
using System.Data;
using System.Reflection;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;
using Roottech.Tracking.Domain.MRAT.Entities;

namespace Roottech.Tracking.Common.Session
{
    public class NHibernateHelper
    {
        private readonly string _connectionString;
        private readonly string _clientDatabaseName;
        private static ISessionFactory _sessionFactory;
        protected static Configuration NhConfiguration;

        public ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null) return _sessionFactory = CreateSessionFactory();
                return (((SessionFactoryImpl)_sessionFactory).Name != _clientDatabaseName)
                           ? _sessionFactory = CreateSessionFactory()
                           : _sessionFactory;
            }
        }

        public NHibernateHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public NHibernateHelper(string connectionString, string clientDatabaseName)
        {
            _connectionString = connectionString + clientDatabaseName;
            _clientDatabaseName = clientDatabaseName;
        }

        private ISessionFactory CreateSessionFactory()
        {
            NhConfiguration = ConfigureNHibernate();
            NhConfiguration.AddMapping(GetMappings());
            SchemaMetadataUpdater.QuoteTableAndColumns(NhConfiguration);
            NhConfiguration.Properties[Environment.CurrentSessionContextClass] = typeof(LazySessionContext).AssemblyQualifiedName;
            NhConfiguration.Proxy(p => p.ProxyFactoryFactory<DefaultProxyFactoryFactory>());
            //ConfigureCaching();
            //ConfigureFilterDefinitions();
            return NhConfiguration.BuildSessionFactory();
        }

        private void ConfigureFilterDefinitions()
        {
            var nominalCodeFilterDef = new FilterDefinition("nominalCodeFilterName", null, // or your default condition
                new Dictionary<string, IType> { { "nominalCodeFilterParamName", NHibernateUtil.String } }, true);
            if (NhConfiguration.FilterDefinitions.ContainsKey(nominalCodeFilterDef.FilterName))
                NhConfiguration.FilterDefinitions.Remove(nominalCodeFilterDef.FilterName);
            NhConfiguration.AddFilterDefinition(nominalCodeFilterDef);
        }

        private void ConfigureCaching()
        {
            const int secondsCacheToExpiration = 86400;
            NhConfiguration.Cache(x =>
            {
                //x.Provider<SysCacheProvider>();
                x.UseMinimalPuts = true;
                x.UseQueryCache = true;
                x.DefaultExpiration = secondsCacheToExpiration;
                x.RegionsPrefix = _clientDatabaseName;
                //http://ayende.com/blog/1708/nhibernate-caching-the-secong-level-cache-space-is-shared
            });
            NhConfiguration.SessionFactory();//.Caching.Through<SysCacheProvider>().WithDefaultExpiration(secondsCacheToExpiration);
        }

        private Configuration ConfigureNHibernate()
        {
            return new Configuration()
                //For Mapping by XML to include Stored Procedures
                //only those assemblies which have queries entites.
            .AddAssembly(Assembly.GetAssembly(typeof(Asset)))
            .Configure()
            .SessionFactoryName(_clientDatabaseName).DataBaseIntegration(db =>
            {
                db.Dialect<MyMsSql2012Dialect>();
                db.Driver<SqlClientDriver>();
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                //db.SchemaAction = SchemaAutoAction.Create;
                db.IsolationLevel = IsolationLevel.ReadCommitted;
                db.ConnectionStringName = _clientDatabaseName;
                db.ConnectionString = _connectionString;
                db.Timeout = 10;
                // batching for large amount of data like 10000 records will take 2000 round trip to database on flush
                db.BatchSize = 5;
                //db.Batcher<>();

                // enabled for testing
                db.LogFormattedSql = true;
                db.LogSqlInConsole = true;
                db.AutoCommentSql = false;
            }).CurrentSessionContext<LazySessionContext>();
            // we are now ready to work with NHibernate
        }

        protected HbmMapping GetMappings()
        {
            //There is a dynamic way to do this, but for simplicity I chose to hard code
            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetAssembly(typeof(Asset)).GetExportedTypes());
            return mapper.CompileMappingForAllExplicitlyAddedEntities();
        }
    }

    public class MyMsSql2012Dialect : MsSql2012Dialect
    {
        public MyMsSql2012Dialect()
        {
            RegisterFunction("FN_GetDIArray", new StandardSQLFunction("dbo.FN_GetDIArray", NHibernateUtil.String));
            RegisterFunction("FN_GetDIArray_descr", new StandardSQLFunction("dbo.FN_GetDIArray_descr", NHibernateUtil.String));
            RegisterFunction("FN_GetEventType", new StandardSQLFunction("dbo.FN_GetEventType", NHibernateUtil.String));
            RegisterFunction("FN_GetSiteType", new StandardSQLFunction("dbo.FN_GetSiteType", NHibernateUtil.String));
            RegisterFunction("FN_GetDuration", new StandardSQLFunction("dbo.FN_GetDuration", NHibernateUtil.String));
            RegisterFunction("FN_GetDateOnly", new SQLFunctionTemplate(NHibernateUtil.Class, "convert(varchar, ?1, 105)"));
            RegisterFunction("FN_GetTimeOnly", new SQLFunctionTemplate(NHibernateUtil.Class, "convert(varchar(8), ?1, 108)"));
            RegisterFunction("FN_GetOpenFuelBalance", new StandardSQLFunction("dbo.FN_GetOpenFuelBalance", NHibernateUtil.Double));
            RegisterFunction("FN_GetCloseFuelBalance", new StandardSQLFunction("dbo.FN_GetCloseFuelBalance", NHibernateUtil.Double));
        }
    }
}