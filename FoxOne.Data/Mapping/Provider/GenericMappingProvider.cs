using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FoxOne.Data.Sql;
using FoxOne.Core;
namespace FoxOne.Data.Mapping.Provider
{
    public abstract class GenericMappingProvider : IMappingProvider
    {

        #region 抽象方法，需要子类实现
        protected abstract string GetTablesSql();
        protected abstract string GetColumnsSql();
        protected abstract string GetKeysSql();
        public abstract bool Supports(string dbProviderName);
        protected abstract string EscapeIdentifier(string name);

        public abstract string CreateTableCommand(Table mapping);

        public abstract string GetDropTableCommand(Table table);

        private string GetFields(TableMapping mapping)
        {
            List<string> field = new List<string>();
            mapping.Table.Columns.ForEach(o =>
            {
                field.Add(EscapeIdentifier(o.Name));
            });
            return string.Join(",\n", field.ToArray());
        }

        public virtual string CreateSelectStatement(TableMapping mapping)
        {
            string selectSQL = "SELECT {0} \n FROM {1} WHERE 1=1 {2}";
            return string.Format(selectSQL, GetFields(mapping), mapping.Table.Name, GetSearchCondition(mapping));
        }

        public virtual string CreateInsertStatement(TableMapping mapping)
        {
            List<string> fieldString = new List<string>();
            List<string> values = new List<string>();
            var fields = mapping.Table.Columns;
            foreach (var field in fields)
            {
                if (!field.IsAutoIncrement)
                {
                    values.Add(string.Format("#{0}#", field.Name));
                    fieldString.Add(EscapeIdentifier(field.Name));
                }
            }
            return string.Format("INSERT INTO {0} \n({1}) \n VALUES \n ({2})", mapping.Table.Name, string.Join(",", fieldString.ToArray()), string.Join(",", values.ToArray()));
        }

        public virtual string CreateUpdateStatement(TableMapping mapping)
        {
            if (mapping.Table.Keys.IsNullOrEmpty())
            {
                return string.Empty;
            }
            string updateSQL = "UPDATE {0} SET {1} WHERE {2}";
            List<string> condition = new List<string>();
            var fields = mapping.Table.Columns;
            string keyName = mapping.Table.Keys.FirstOrDefault().Name;
            condition.Add(string.Format("{0}=#{1}#\n", EscapeIdentifier(keyName), keyName));
            foreach (var field in fields)
            {
                if (!field.IsKey)
                {
                    condition.Add(string.Format("{{?? ,{0}=#{1}# }}\n", EscapeIdentifier(field.Name), field.Name));
                }
            }
            return string.Format(updateSQL, mapping.Table.Name, string.Join("", condition.ToArray()), GetWhereCondition(mapping));
        }

        protected virtual string GetWhereCondition(TableMapping mapping)
        {
            List<string> condition = new List<string>();
            foreach (var field in mapping.Table.Keys)
            {
                condition.Add(string.Format("{0}=#{1}#", EscapeIdentifier(field.Name), field.Name));
            }
            return string.Join(" AND ", condition.ToArray());
        }

        protected abstract string GetSearchCondition(TableMapping mapping);

        public virtual string CreateDeleteStatement(TableMapping mapping)
        {
            string deleteSQL = "DELETE FROM {0} WHERE {1}";
            if (mapping.Table.Keys.Count > 1)
            {
                return string.Format(deleteSQL, mapping.Table.Name, GetWhereCondition(mapping));
            }
            else
            {
                return string.Format(deleteSQL, mapping.Table.Name, "{0} IN (${0}$)".FormatTo(mapping.Table.Keys.FirstOrDefault().Name));
            }
        }

        public virtual string CreateGetOneStatement(TableMapping mapping)
        {
            string selectOneSQL = "SELECT {0} \n FROM {1} WHERE {2}";
            return string.Format(selectOneSQL, GetFields(mapping), mapping.Table.Name, GetWhereCondition(mapping));
        }
        #endregion

        #region 读取数据库信息通用实现
        public virtual IList<Table> ReadTables(Dao dao)
        {
            IList<Table> tables = dao.QueryEntities<Table>(GetTablesSql());
            if (tables.Count > 0)
            {
                IEnumerable<Column> columns = ReadColumns(dao);
                IEnumerable<Key> keys = ReadKeys(dao);
                tables.ForEach(table =>
                {
                    columns.ForEach(column =>
                    {
                        if (column.Table.Equals(table.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            if (string.IsNullOrEmpty(table.Schema) ||
                                table.Schema.Equals(column.Schema, StringComparison.OrdinalIgnoreCase))
                            {
                                table.AddColumn(column);
                            }
                            else
                            {
                                Logger.Debug("schema name '{0}' and '{1}' not matched between table '{2}' and column '{3}'",
                                          table.Schema, column.Schema, table.Name, column.Name);
                            }
                        }
                    });

                    keys.ForEach(key =>
                    {
                        if (key.Table.Equals(table.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            if (string.IsNullOrEmpty(table.Schema) ||
                                table.Schema.Equals(key.Schema, StringComparison.OrdinalIgnoreCase))
                            {
                                Column keyColumn = table.GetColumn(key.Column);

                                if (null == keyColumn)
                                {
                                    throw new FoxOneException(string.Format("key column '{0}' not exist in table '{1}'", key.Column, table.Name));
                                }

                                keyColumn.IsKey = true;
                                table.AddKey(keyColumn);
                            }
                            else
                            {
                                Logger.Debug("schema name '{0}' and '{1}' not matched between table '{2}' and key '{3}'",
                                          table.Schema, key.Schema, table.Name, key.Column);
                            }
                        }
                    });
                });
            }
            return tables;
        }


        protected IEnumerable<Column> ReadColumns(Dao dao)
        {
            return dao.QueryEntities<Column>(GetColumnsSql());
        }

        protected IEnumerable<Key> ReadKeys(Dao dao)
        {
            return dao.QueryEntities<Key>(GetKeysSql());
        }
        #endregion

        #region 构造SQL语句通用实现
        public ISqlCommand CreateSelectCommand(TableMapping mapping, object parameters)
        {
            return CreateSelectOrCountCommand(mapping, parameters);
        }

        public ISqlCommand CreateSelectCountCommand(TableMapping mapping, object parameters)
        {
            return CreateSelectOrCountCommand(mapping, parameters, true);
        }

        private const string SELECT_COMMAND = "SELECT * FROM ";
        private const string SELECT_COUNT_COMMAND = "SELECT COUNT(0) FROM ";
        private const string INSERT_COMMAND = "INSERT INTO ";
        private const string DELETE_COMMAND = "DELETE FROM ";
        private const string UPDATE_COMMAND = "UPDATE ";
        private const string WHERE_STATEMENT = " WHERE ";

        private ISqlCommand CreateSelectOrCountCommand(TableMapping mapping, object parameters, bool forSelectCount = false)
        {
            SqlCommandBuilder sql = new SqlCommandBuilder();

            sql.AppendCommandText(forSelectCount ? SELECT_COUNT_COMMAND : SELECT_COMMAND);
            sql.AppendCommandText(EscapeTableName(mapping.Table.Name))
               .AppendCommandText(WHERE_STATEMENT);

            AppendKeyWheres(sql, mapping, GetKeyParams(mapping, parameters));

            return sql.ToCommand();
        }

        public ISqlCommand CreateSelectAllCommand(TableMapping mapping, object parameter)
        {
            SqlCommandBuilder sql = new SqlCommandBuilder();
            sql.AppendCommandText(SELECT_COMMAND)
               .AppendCommandText(EscapeTableName(mapping.Table.Name));
            if (parameter != null)
            {
                sql.AppendCommandText(WHERE_STATEMENT);
                AppendWhere(sql, mapping, parameter);
            }
            return sql.ToCommand();
        }

        public ISqlCommand CreateInsertCommand(TableMapping mapping, object entity)
        {
            ISqlParameters parameters = new MappingParameters(mapping, entity);
            SqlCommandBuilder sql = new SqlCommandBuilder();

            StringBuilder names = new StringBuilder();
            StringBuilder values = new StringBuilder();

            int index = 0;
            mapping.InsertColumns.ForEach(col =>
                {
                    //自增长列不需要作为插入的字段
                    if (!col.IsAutoIncrement)
                    {
                        object value;

                        //TODO : 如果该字段没有找到对应的参数，则不作为插入的字段？
                        if (parameters.TryResolve(col.Name, out value))
                        {
                            if (index > 0)
                            {
                                names.Append(",");
                                values.Append(",");
                            }
                            index++;

                            string paramName = EscapeParamName(col.Name);

                            names.Append(EscapeIdentifier(col.Name));
                            values.Append(string.Format(NamedParameterFormat, paramName));

                            sql.AddCommandParameter(paramName, value);
                        }
                    }
                });

            sql.AppendCommandText(INSERT_COMMAND)
               .AppendCommandText(EscapeTableName(mapping.Table.Name))
               .AppendCommandText(" (")
               .AppendCommandText(names.ToString())
               .AppendCommandText(") Values (")
               .AppendCommandText(values.ToString())
               .AppendCommandText(")");

            return sql.ToCommand();
        }

        public ISqlCommand CreateUpdateCommand(TableMapping mapping, object entity)
        {
            return CreateUpdateCommand(mapping, entity, null, true);
        }

        public ISqlCommand CreateUpdateCommand(TableMapping mapping, object entity, string[] fields, bool inclusive)
        {
            ISqlParameters parameters = new MappingParameters(mapping, entity);
            SqlCommandBuilder sql = new SqlCommandBuilder();

            sql.AppendCommandText(UPDATE_COMMAND)
               .AppendCommandText(EscapeTableName(mapping.Table.Name))
               .AppendCommandText(" Set ");

            int index = 0;
            mapping.UpdateColumns.ForEach(col =>
            {
                //更新时忽略主键字段
                if (!col.IsKey && (null == fields || fields.Length == 0 || (inclusive && contains(col, fields)) || !inclusive && !contains(col, fields)))
                {
                    object value;

                    if (parameters.TryResolve(col.Name, out value))
                    {
                        if (index > 0)
                        {
                            sql.AppendCommandText(",");
                        }
                        index++;

                        string paramName = EscapeParamName(col.Name);

                        sql.AppendCommandText(EscapeIdentifier(col.Name))
                           .AppendCommandText("=")
                           .AppendCommandText(string.Format(NamedParameterFormat, paramName));

                        sql.AddCommandParameter(paramName, value);
                    }
                }
            });

            sql.AppendCommandText(WHERE_STATEMENT);

            AppendKeyWheres(sql, mapping, entity);

            return sql.ToCommand();
        }



        private bool contains(Column col, String[] fields)
        {
            bool contains = false;
            if (null != fields)
            {
                foreach (String field in fields)
                {
                    if (col.Name.Equals(field, StringComparison.OrdinalIgnoreCase) || (col.Property != null && col.Property.Name.Equals(field, StringComparison.OrdinalIgnoreCase)))
                    {
                        contains = true;
                        break;
                    }
                }
            }
            return contains;
        }

        public virtual ISqlCommand CreateDeleteCommand(TableMapping mapping, object id)
        {
            SqlCommandBuilder sql = new SqlCommandBuilder();

            sql.AppendCommandText(DELETE_COMMAND)
               .AppendCommandText(EscapeTableName(mapping.Table.Name))
               .AppendCommandText(WHERE_STATEMENT);

            AppendKeyWheres(sql, mapping, GetKeyParams(mapping, id));

            return sql.ToCommand();
        }

        protected virtual object GetKeyParams(TableMapping mapping, object id)
        {
            int tableKeyCount = mapping.Table.Keys.Count;

            if (tableKeyCount == 1)
            {
                //TODO Some Single Value Type is No Primitive or Value Type , Now Simple Implement With Special Compare
                if (id.GetType().Equals(typeof(string)) || id.GetType().IsPrimitive || id.GetType().IsValueType)
                {
                    return new Dictionary<string, object>
                           {
                               {mapping.Table.Keys[0].Name, id}
                           };
                }
                else
                {
                    return id;
                }
            }
            else if (tableKeyCount > 1)
            {
                if (id.GetType().Equals(typeof(string)) || id.GetType().IsPrimitive || id.GetType().IsValueType)
                {
                    throw new FoxOneException("table {0} has more than one field as key, so you must pass more then one param by Dictonary or Entity Class", mapping.Table.Name);
                }
                else
                {
                    return id;
                }
            }

            if (tableKeyCount == 0)
            {
                throw new FoxOneException("the key of table {0} is absent, so you can't call dao's select or delete method", mapping.Table.Name);
            }
            return id;
        }

        protected virtual void AppendWhere(SqlCommandBuilder sql, TableMapping mapping, object parameter)
        {
            Table table = mapping.Table;
            IDictionary<string, object> param = parameter as IDictionary<string, object>;
            if (param == null)
            {
                param = parameter.ToDictionary();
            }
            int index = 0;
            string operation = string.Empty;
            foreach (var key in param.Keys)
            {
                if (table.Columns.FirstOrDefault(o => o.Name.Equals(key, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    continue;
                }
                if (param[key] == null || param[key].ToString().IsNullOrEmpty())
                {
                    continue;
                }
                if (index > 0)
                {
                    sql.AppendCommandText(" AND ");
                }
                index++;
                string paramName = EscapeParamName(key);
                object value = param[key];
                if (param[key].GetType().IsArray)
                {
                    value = "'{0}'".FormatTo(String.Join("','", param[key] as string[]));
                    operation = "{0} IN ({1})".FormatTo(EscapeIdentifier(key), value);
                }
                else
                {
                    operation = "{0} = {1}".FormatTo(EscapeIdentifier(key), string.Format(NamedParameterFormat, paramName));
                    sql.AddCommandParameter(paramName, value);
                }
                sql.AppendCommandText(operation);
            }
        }

        protected virtual void AppendKeyWheres(SqlCommandBuilder sql, TableMapping mapping, object idparams)
        {
            Table table = mapping.Table;
            ISqlParameters parameters = new MappingParameters(mapping, idparams);
            var column = table.Keys;
            int index = 0;
            column.ForEach(key =>
            {
                if (index > 0)
                {
                    sql.AppendCommandText(" and ");
                }
                index++;

                string paramName = EscapeParamName(key.Name);

                sql.AppendCommandText(EscapeIdentifier(key.Name))
                   .AppendCommandText("=")
                   .AppendCommandText(string.Format(NamedParameterFormat, paramName));

                object value;
                if (parameters.TryResolve(key.Name, out value))
                {
                    sql.AddCommandParameter(paramName, value);
                }
                else
                {
                    throw new FoxOneException(string.Format("parameter value of primary key '{0}' not found", key.Name));
                }
            });
        }

        protected virtual string EscapeTableName(string name)
        {
            return EscapeIdentifier(name);
        }

        protected virtual string EscapeParamName(string name)
        {
            return name.Replace(" ", "");
        }

        protected abstract string NamedParameterFormat { get; }
        #endregion

        protected class Key
        {
            public String Schema { get; set; }
            public String Table { get; set; }
            public String Column { get; set; }
        }


        public ISqlCommand CreateBatchUpdateCommand(TableMapping mapping, object entity, object whereParameter)
        {
            IDictionary<string, object> parameter = entity as IDictionary<string, object>;
            if (parameter == null)
            {
                parameter = entity.ToDictionary();
            }
            SqlCommandBuilder sql = new SqlCommandBuilder();

            sql.AppendCommandText(UPDATE_COMMAND)
               .AppendCommandText(EscapeTableName(mapping.Table.Name))
               .AppendCommandText(" Set ");

            int index = 0;
            foreach (var key in parameter.Keys)
            {
                if (mapping.UpdateColumns.FirstOrDefault(o => o.Name.Equals(key, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    continue;
                }
                if (parameter[key] == null || parameter[key].ToString().IsNullOrEmpty())
                {
                    continue;
                }
                if (index > 0)
                {
                    sql.AppendCommandText(",");
                }
                index++;

                string paramName = EscapeParamName(key);

                sql.AppendCommandText(EscapeIdentifier(key))
                   .AppendCommandText("=")
                   .AppendCommandText(string.Format(NamedParameterFormat, paramName));

                sql.AddCommandParameter(paramName, parameter[key]);
            }
            sql.AppendCommandText(WHERE_STATEMENT);
            AppendWhere(sql, mapping, whereParameter);
            return sql.ToCommand();
        }

        public ISqlCommand CreateBatchDeleteCommand(TableMapping mapping, object whereParameter)
        {
            SqlCommandBuilder sql = new SqlCommandBuilder();

            sql.AppendCommandText(DELETE_COMMAND)
               .AppendCommandText(EscapeTableName(mapping.Table.Name));
            if (whereParameter != null)
            {
                sql.AppendCommandText(WHERE_STATEMENT);
                AppendWhere(sql, mapping, whereParameter);
            }

            return sql.ToCommand();
        }
    }

    internal class MappingParameters : BaseParameters
    {
        private readonly TableMapping _mapping;
        private readonly SqlParameters _sqlParams;
        private readonly object _rawParams;

        internal MappingParameters(TableMapping mapping, object parameters)
        {
            this._mapping = mapping;
            this._rawParams = parameters;
            this._sqlParams = new SqlParameters(parameters);
        }

        public override bool TryResolve(string name, out object value)
        {
            //这里传入的name是字段名，根据字段名找到映射的属性
            FastProperty prop = _mapping.GetMappingProperty(name);

            //如果参数是实体对象，直接返回属性的值
            if (null != prop && _sqlParams.IsObjectParameters() && prop.Info.DeclaringType.IsInstanceOfType(_rawParams))
            {
                value = prop.GetValue(_rawParams);
                return true;
            }
            //先找字段名
            if (!_sqlParams.TryResolve(name, out value))
            {
                //再找属性名
                if (null != prop && !prop.Name.Equals(name))
                {
                    return _sqlParams.TryResolve(prop.Name, out value);
                }
                return false;
            }
            return true;
        }
    }
}