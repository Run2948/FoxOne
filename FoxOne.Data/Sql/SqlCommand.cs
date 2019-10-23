using System.Collections.Generic;
using FoxOne.Data.Provider;

namespace FoxOne.Data.Sql
{
    /// <summary>
    /// 表示一个最终被数据库访问框架执行的数据库SQL命名
    /// </summary>
    public class SqlCommand : ISqlCommand
    {
        private string _commandText;
        private IList<KeyValuePair<string, object>> _parameters;

        protected SqlCommand()
        {
            
        }

        public SqlCommand(string commandText,IList<KeyValuePair<string,object>> parameters)
        {
            _commandText = commandText;
            _parameters  = parameters;
        }

        /// <summary>
        /// SQL语句
        /// </summary>
        public virtual string CommandText
        {
            get { return _commandText; }
            protected set { _commandText = value; }
        }

        /// <summary>
        /// SQL语句中的命名参数对应的参数集合
        /// </summary>
        public virtual IList<KeyValuePair<string, object>> Parameters
        {
            get { return _parameters; }
            protected set { _parameters = value; }
        }
    }
}