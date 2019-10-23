using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FoxOne.Data.Provider
{
    public abstract class DaoProvider : IDaoProvider
    {
        internal const string OrderByClausePatterString = @"[{]\s*[?]\s*order\s+by[\d|\w|\s|$|#|@|:|-|_]*[}]|order\s+by[\d|\w|\s|$|#|@|:|-|_]*";
        static DaoProvider()
        {

        }
        private bool _supportsNamedParameter = true;
        private string _namedParameterFormat;
        private string _name;

        protected DaoProvider(string name)
        {
            _name = name;
        }

        protected DaoProvider(string name, string namedParameterFormat)
        {
            _name = name;
            _namedParameterFormat = namedParameterFormat;
        }

        protected DaoProvider(string name, string namedParameterFormat, bool supportsNamedParameter)
            : this(name, namedParameterFormat)
        {
            _supportsNamedParameter = supportsNamedParameter;
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual bool SupportsDbProvider(string dbProviderName)
        {
            return dbProviderName.Equals(Name, StringComparison.OrdinalIgnoreCase);
        }

        public virtual bool SupportsNamedParameter
        {
            get { return _supportsNamedParameter; }
            protected set { _supportsNamedParameter = value; }
        }

        public virtual bool NamedParameterMustOneByOne
        {
            get { return false; }
        }

        public virtual string NamedParameterFormat
        {
            get { return _namedParameterFormat; }
            protected set { _namedParameterFormat = value; }
        }

        public abstract string WrapPageSql(string sql, string orderClause, int startRowIndex, int rowCount, out IDictionary<string, object> pageParam);

        public virtual string WrapCountSql(string sql)
        {
            sql = RemoveOrderByClause(sql);
            return " select count(1) from (\n" + sql + "\n) tt";
        }

        public virtual string EscapeText(string text)
        {
            return null == text ? text : text.Replace("'", "''");
        }

        /// <summary>
        /// 由字段名生成SQL参数名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string EscapeParam(string name)
        {
            // 为参数统一增加后缀"_"，在Oracle数据库中，命名参数为关键字时，会执行报错
            return name.Replace(":", "_").Replace(".", "__").Replace("@", "_") + "_";
        }

        public virtual string EscapeLikeParamValue(string value)
        {
            return value;
        }

        public virtual string PageParamNameBegin
        {
            get
            {
                return "Row__Begin";
            }
        }

        public virtual string PageParamNameEnd
        {
            get
            {
                return "Row__End";
            }
        }

        protected string RemoveOrderByClause(string sql)
        {
            Regex OrderByClausePattern = new Regex(OrderByClausePatterString, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            sql = OrderByClausePattern.Replace(sql.Trim(), String.Empty);
            return sql;
        }
    }
}
