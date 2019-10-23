using System.Collections.Generic;
using System.Text;

namespace FoxOne.Data.Sql
{
    public class SqlCommandBuilder
    {
        private readonly StringBuilder _sql = new StringBuilder();
        private readonly IList<KeyValuePair<string, object>> _params = new List<KeyValuePair<string, object>>();

        internal SqlCommandBuilder()
        {
            
        }

        public StringBuilder CommandText
        {
            get { return _sql; }
        }

        public IList<KeyValuePair<string,object>> Params
        {
            get { return _params; }
        }

        public SqlCommandBuilder AppendCommandText(string sql)
        {
            _sql.Append(sql);
            return this;
        }

        public SqlCommandBuilder AddCommandParameter(string name, object value)
        {
            _params.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }

        public SqlCommandBuilder AddCommandParameter(KeyValuePair<string,object> param)
        {
            _params.Add(param);
            return this;
        }

        public SqlCommand ToCommand()
        {
            return new SqlCommand( _sql.ToString().Trim(), _params);
        }
    }
}