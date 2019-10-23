using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;
using FoxOne.Data.Mapping;
using FoxOne.Data.Provider;
using FoxOne.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data.SqlCe;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using FoxOne.Core;
using FoxOne.Data.MySql;
using System.Collections.Concurrent;
namespace FoxOne.Data
{
    public sealed class DaoFactory
    {
        public const string DefaultConnectionName = "DefaultDB";

        private static ISqlSource _sqlSource;
        private static IDictionary<string, Dao> _daos;
        private static IDictionary<string, IDaoProvider> _providers;
        private static IEnumerable<ISqlParameters> _parameters;
        private static IDictionary<string, IDaoProvider> _providerMapping;
        private static IEnumerable<ISqlActionExecutor> _actionExecutors;
        private static LinkedList<IMappingProvider> _mappingProviders;

        static DaoFactory()
        {
            Initialize();
        }

        public static IEnumerable<ISqlParameters> Parameters
        {
            get { return _parameters; }
        }

        public static Dao GetDao()
        {
            return GetDao(DefaultConnectionName);
        }

        public static Dao GetDao(String name)
        {
            Dao dao;
            if (!_daos.TryGetValue(name, out dao))
            {
                dao = new DatabaseDao(name, CreateDatabase(name));
                _daos.Add(name, dao);
            }
            return dao;
        }

        public static Dao GetDao(String connectionString, String providerName)
        {
            Dao dao;
            if (!_daos.TryGetValue(connectionString, out dao))
            {
                dao = new DatabaseDao(connectionString, CreateDatabase(connectionString, providerName), providerName);
                _daos.Add(connectionString, dao);

            }
            return dao;
        }

        public static Database GetDatabase()
        {
            return GetDatabase(DefaultConnectionName);
        }
        public static Database GetDatabase(string name)
        {
            return GetDao(name).Database;
        }

        public static ISqlSource GetSqlSource()
        {
            return _sqlSource;
        }

        public static IEnumerable<ISqlActionExecutor> ActionExecutors
        {
            get { return _actionExecutors; }
        }

        internal static IDaoProvider GetDaoProvider(string providerName)
        {
            return _providers.Values.SingleOrDefault(p => p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase));
        }

        internal static IDaoProvider GetDaoProviderOfDbProvider(string dbProviderName)
        {
            IDaoProvider provider;
            if (!_providerMapping.TryGetValue(dbProviderName, out provider))
            {
                provider = _providers.Values.SingleOrDefault(p => p.SupportsDbProvider(dbProviderName));
                if (null != provider)
                {
                    _providerMapping.Add(dbProviderName, provider);
                }
            }
            return provider;
        }

        /// <summary>
        /// 根据DbProvider的名称返回对应的IMappingProvider
        /// </summary>
        /// <param name="dbProviderName">连接串配置中的provider name</param>
        public static IMappingProvider GetMappingProvider(string dbProviderName)
        {
            return _mappingProviders.Single(r => r.Supports(dbProviderName));
        }

        /// <summary>
        /// 根据连接串名字创建企业库Database对象
        /// </summary>
        /// <param name="name">连接串参数</param>
        /// <returns></returns>
        private static Database CreateDatabase(string name)
        {
            try
            {
                return DatabaseFactory.CreateDatabase(name);
            }
            catch (ResolutionFailedException e)
            {
                throw new FoxOneException(string.Format("Please check is the connection string '{0}' exists and correct", name), e);
            }
            catch (ActivationException e)
            {
                throw new FoxOneException(string.Format("Please check is the connection string '{0}' exists and correct", name), e);
            }
        }

        /// <summary>
        /// 根据连接串和数据库提供者，创建企业库Database对象
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="providerName">数据库提供者</param>
        /// <returns>企业库Database对象</returns>
        private static Database CreateDatabase(string connectionString, string providerName)
        {
            if (String.IsNullOrEmpty(connectionString) || String.IsNullOrEmpty(providerName))
            {
                throw new FoxOneException("When CreateDatabase, connectionString and providerName must pass in");
            }

            if ("System.Data.SqlClient".Equals(providerName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return new SqlDatabase(connectionString);
            }

            if ("System.Data.OracleClient".Equals(providerName.Trim(), StringComparison.OrdinalIgnoreCase) || "Oracle.DataAccess.Client".Equals(providerName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return new OracleDatabase(connectionString);
            }

            if (providerName.StartsWith("System.Data.SqlServerCe", StringComparison.OrdinalIgnoreCase))
            {
                return new SqlCeDatabase(connectionString);
            }

            if (providerName.StartsWith("MySql.Data.MySqlClient", StringComparison.OrdinalIgnoreCase))
            {
                return new MySqlDatabase(connectionString);
            }

            throw new FoxOneException(String.Format("please check provider name '{0}' is correct,now only suport sql server,oracle,sql server ce", providerName));
        }

        private static void Initialize()
        {
            //必须先初始化ActionExecutor，因为在加载Sql时需要引用
            _actionExecutors = TypeHelper.GetAllImplInstance<ISqlActionExecutor>();
            _parameters = ObjectHelper.GetAllObjects<ISqlParameters>();


            InitializeProviders();
            InitializeMappingProviders();

            _daos = new ConcurrentDictionary<string, Dao>();
            _sqlSource = new SqlSource().LoadSqls();
        }

        private static void InitializeProviders()
        {
            _providers = new ConcurrentDictionary<string, IDaoProvider>();
            _providerMapping = new ConcurrentDictionary<string, IDaoProvider>();
            TypeHelper.GetAllImplInstance<IDaoProvider>().ForEach(provider => _providers.Add(provider.Name, provider));
        }

        private static void InitializeMappingProviders()
        {
            _mappingProviders = new LinkedList<IMappingProvider>();
            TypeHelper.GetAllImplInstance<IMappingProvider>().ForEach(provider => _mappingProviders.AddFirst(provider));
        }
    }
}