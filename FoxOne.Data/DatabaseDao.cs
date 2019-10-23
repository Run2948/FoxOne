using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;
using FoxOne.Core;
using System;
namespace FoxOne.Data
{
    /// <summary>
    /// 使用企业库数据访问框架的<c>Dao</c>实现
    /// </summary>
    public class DatabaseDao : GenericDao
    {
        private readonly Database _database;

        public DatabaseDao()
            : this(DaoFactory.DefaultConnectionName)
        {

        }

        public DatabaseDao(string name)
            : base(name)
        {
            _database = DaoFactory.GetDatabase(name);
        }

        public DatabaseDao(string name, Database db)
            : base(name)
        {
            _database = db;
        }

        public DatabaseDao(string name, Database db, string providerName)
            : base(name, providerName)
        {
            _database = db;
            _connectionString = db.ConnectionString;
        }

        public override Database Database
        {
            get { return _database; }
        }

        protected Database GetDatabase(string connectionName)
        {
            return string.IsNullOrEmpty(connectionName) ? _database : DaoFactory.GetDatabase(connectionName);
        }

        protected override DbCommand CreateDbCommand(string sql, string connectionName)
        {
            return GetDatabase(connectionName).GetSqlStringCommand(sql);
        }

        protected override int ExecuteNonQuery(DbCommand command, string connectionName)
        {
            return GetDatabase(connectionName).ExecuteNonQuery(command);
        }

        protected override IDataReader ExecuteReader(DbCommand command, string connectionName)
        {
            return GetDatabase(connectionName).ExecuteReader(command);
        }

        protected override DataSet ExecuteDataSet(DbCommand command, string connectionName)
        {
            return GetDatabase(connectionName).ExecuteDataSet(command);
        }

        protected override object ExecuteScalar(DbCommand command, string connectionName)
        {
            return GetDatabase(connectionName).ExecuteScalar(command);
        }

        public override int ExecuteProcedure(string procedureName, params object[] parameters)
        {
            return _database.ExecuteNonQuery(procedureName, parameters);
        }

        public override int BatchInsert(DataTable table)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(ConnectionString);
            sqlBulkCopy.DestinationTableName = table.TableName;
            sqlBulkCopy.BatchSize = table.Rows.Count;
            foreach (DataColumn item in table.Columns)
            {
                sqlBulkCopy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
            }
            if (table != null && table.Rows.Count != 0)
            {
                sqlBulkCopy.WriteToServer(table);
            }
            sqlBulkCopy.Close();
            return table.Rows.Count;
        }

        public override int BatchInsert(System.Collections.Generic.IList<System.Collections.Generic.IDictionary<string, object>> data, string tableName)
        {
            if(!data.IsNullOrEmpty())
            {
                DataTable table = new DataTable() { TableName = tableName };
                foreach (var key in data[0].Keys)
                {
                    DataColumn idColumn = new DataColumn(key, data[0][key].GetType());
                    table.Columns.Add(idColumn);
                }
                int i = 0;
                object obj = null;
                foreach(var d in data)
                {
                    DataRow dataRow = table.NewRow();
                    i = 0;
                    foreach(var key in d.Keys)
                    {
                        obj = d[key];
                        if(obj==null)
                        {
                            obj = DBNull.Value;
                        }
                        dataRow[i++] = obj;
                    }
                    table.Rows.Add(dataRow);
                }
                return BatchInsert(table);
            }
            return 0;
        }
    }
}