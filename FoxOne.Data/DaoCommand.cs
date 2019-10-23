using FoxOne.Core;
using System.Data;

namespace FoxOne.Data
{
    public class DaoCommand
    {
        public static int ExecuteNonQuery(string key,object parameters = null)
        {
            DaoExecutor executor = CreateCommand(key, parameters);
            return executor.Dao.ExecuteNonQuery(executor.Command, null);
        }

        public static int ExecuteNonQuery(ISqlStatement sql, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(sql, parameters);
            return executor.Dao.ExecuteNonQuery(executor.Command, null);
        }

        public static IDataReader QueryReader(string key, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(key, parameters);
            return executor.Dao.QueryReader(executor.Command, null);
        }

        public static IDataReader QueryReader(ISqlStatement sql, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(sql, parameters);
            return executor.Dao.QueryReader(executor.Command, null);
        }

        public static DataSet QueryDataSet(string key, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(key, parameters);
            return executor.Dao.QueryDataSet(executor.Command, null);
        }

        public static T QueryScalar<T>(string key, object parameters = null)
        {
            DaoExecutor executor = CreateCommand(key, parameters);
            return executor.Dao.QueryScalar<T>(executor.Command, null);
        }

        private static DaoExecutor CreateCommand(string key, object parameters)
        {
            ISqlStatement sql = DaoFactory.GetSqlSource().Find(key, Dao.Get().Provider.Name);
            if (null == sql)
            {
                throw new FoxOneException(string.Format("sql command '{0}' not found",key));
            }

            Dao dao = string.IsNullOrEmpty(sql.Connection) ? Dao.Get() : Dao.Get(sql.Connection);            

            return new DaoExecutor { Command = sql.CreateCommand(dao.Provider, parameters) , Dao = dao};
        }

        private static DaoExecutor CreateCommand(ISqlStatement sql, object parameters)
        {
            Dao dao = string.IsNullOrEmpty(sql.Connection) ? Dao.Get() : Dao.Get(sql.Connection);

            ISqlCommand command = sql.CreateCommand(dao.Provider, parameters);

            return new DaoExecutor { Command = command, Dao = dao };
        }

        internal struct DaoExecutor
        {
            internal ISqlCommand Command { get; set; }
            internal Dao Dao { get; set; }
        }
    }
}
