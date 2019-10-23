using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using FoxOne.Core;
namespace FoxOne.Data.Sql
{
    /// <summary>
    /// SQL分句
    /// </summary>
    public abstract class SqlClause
    {
        public string RawText { get; protected set; }

        /// <summary>
        /// </summary>
        /// <returns>返回是否被执行，有些语句是根据上下文动态判断是否需要被执行的</returns>
        public abstract bool ToCommand(IDaoProvider provider, SqlCommandBuilder builder, ISqlParameters parameters);
    }

    public class TextClause : SqlClause
    {

        public TextClause(string rawText)
        {
            RawText = rawText;
        }

        public override bool ToCommand(IDaoProvider provider, SqlCommandBuilder builder, ISqlParameters parameters)
        {
            builder.AppendCommandText(RawText);

            return true;
        }
    }

    public abstract class ParameterClause : SqlClause
    {
        protected ParameterClause(string rawText, string name)
        {
            RawText = rawText;
            ParamName = name;
        }

        public string ParamName { get; protected set; }
    }

    public class NamedParameterClause : ParameterClause
    {
        public NamedParameterClause(string rawText, string name)
            : base(rawText, name)
        {

        }

        public override bool ToCommand(IDaoProvider provider, SqlCommandBuilder builder, ISqlParameters parameters)
        {
            string sqlName = provider.EscapeParam(ParamName);
            builder.AppendCommandText(string.Format(provider.NamedParameterFormat, sqlName));
            builder.AddCommandParameter(sqlName, parameters.Resolve(ParamName));

            return true;
        }
    }

    public class DbNamedParameterClause : ParameterClause
    {
        public DbNamedParameterClause(string rawText, string name)
            : base(rawText, name)
        {

        }

        public override bool ToCommand(IDaoProvider provider, SqlCommandBuilder builder, ISqlParameters parameters)
        {
            string sqlName = provider.EscapeParam(ParamName);
            builder.AppendCommandText(RawText.Replace(ParamName, sqlName));
            builder.AddCommandParameter(sqlName, parameters.Resolve(ParamName));

            return true;
        }
    }

    /// <summary>
    /// 值参数传递时，值参数的用途
    /// </summary>
    public enum ValueParameterClauseUsage
    {
        Normal, // 用在等于大于小于等子句中
        In, // 用于In子句中
        Like // 用于Like子句中
    }

    public class ValueParameterClause : ParameterClause
    {
        private readonly string _defaultValue;
        private readonly ValueParameterClauseUsage _valueParameterClauseUsage;

        public ValueParameterClause(string rawText, string name, ValueParameterClauseUsage valueParameterClauseUsage = ValueParameterClauseUsage.Normal)
            : base(rawText, name)
        {
            int index = name.IndexOf('?');
            if (index > 0)
            {
                ParamName = name.Substring(0, index).Trim();
                _defaultValue = name.Length > index + 1 ? name.Substring(index + 1).Trim() : null;
            }
            _valueParameterClauseUsage = valueParameterClauseUsage;
        }

        public override bool ToCommand(IDaoProvider provider, SqlCommandBuilder builder, ISqlParameters parameters)
        {
            object value = parameters.Resolve(ParamName);

            if(_valueParameterClauseUsage == ValueParameterClauseUsage.In)
            {
                builder.AppendCommandText(ToInClauseSqlString(provider, value));
            }
            else
            {
                String content = ToSqlString(provider, value);
                if (String.IsNullOrEmpty(content)&&!String.IsNullOrEmpty(_defaultValue))
                {
                    builder.AppendCommandText(_defaultValue);
                }
                else
                {
                    if (_valueParameterClauseUsage == ValueParameterClauseUsage.Like)
                    {
                        content = provider.EscapeLikeParamValue(content);
                    }
                    builder.AppendCommandText(content);
                }
            }
            return true;
        }

        private string ToInClauseSqlString(IDaoProvider provider, Object value)
        {
            if (null == value)
            {
                return String.IsNullOrEmpty(_defaultValue) ? _defaultValue : "''";
            }

            string content = null;
            if (value is String)
            {
                content = value.ToString().Trim();

                // 如果前后都已经有单引号则直接返回
                if (content.Length >= 2 && content.StartsWith("'") && content.EndsWith("'"))
                {
                    return content;
                }

                content = provider.EscapeText(content);
            }
            else if (value is IEnumerable)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var single in (IEnumerable)value)
                {
                    if (null != single)
                    {
                        builder.Append(",").Append(provider.EscapeText(single.ToString()));
                    }
                }
                content = builder.Length > 0 ? builder.ToString().Substring(1) : "";
                if (String.IsNullOrEmpty(content))
                {
                    return String.IsNullOrEmpty(_defaultValue) ? "" : _defaultValue;
                }
            }

            return ProcessInSqlParam(content);
        }

        //处理In子句中逗号分隔开的字符串信息，对于字符串自动加单引号
        private static string ProcessInSqlParam(string paramValue)
        {
            String[] elements = paramValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //bool isNumeric = true;
            bool ifExistDot = false;
            foreach (string element in elements)
            {
                if (element.StartsWith("'"))
                {
                    ifExistDot = true;
                    break;
                }
            }
            if (!ifExistDot)
            {
                return "'" + String.Join("','", elements) + "'";
            }
            return paramValue;
        }

        private static string ToSqlString(IDaoProvider provider, object value)
        {
            if (null == value || value is DBNull)
            {
                return String.Empty;
            }
            else if (value is string)
            {
                return provider.EscapeText(((string)value).Trim());
            }
            else if (value is IEnumerable)
            {
                StringBuilder builder = new StringBuilder();
                foreach (object single in ((IEnumerable)value))
                {
                    builder.Append(",");
                    builder.Append(ToSqlString(provider, single));
                }
                return builder.Length > 0 ? builder.Remove(0, 1).ToString() : string.Empty;
            }
            else
            {
                return provider.EscapeText(value.ConvertTo<string>().Trim());
            }
        }
    }

    public class DynamicClause : SqlClause
    {
        private IList<SqlClause> _childs;
        private ParameterClause _paramClause;
        private bool _foundNestedDynamicClause;
        private bool _strict = false;

        public DynamicClause(string rawText, string content)
        {
            if (rawText.StartsWith("{??"))
            {
                _strict = true;
                RawText = rawText.Replace("{??", "{?");
                Content = content.Substring(1);
            }
            else
            {
                RawText = rawText;
                Content = content;
            }
            
            
            _childs = ParseChilds();
        }

        public string Content { get; private set; }

        public IList<SqlClause> Childs
        {
            get { return _childs; }
        }

        public override bool ToCommand(IDaoProvider provider, SqlCommandBuilder builder, ISqlParameters parameters)
        {
            if (_paramClause != null)
            {
                object value;
                
                bool exist = parameters.TryResolve(_paramClause.ParamName,out value);

                if (!_strict && (null == value || (value is string && string.IsNullOrEmpty(value as string))))
                {
                    return false;
                }

                if (_strict && !exist)
                {
                    return false;
                }
            }

            if (_foundNestedDynamicClause)
            {
                bool anyNestedDynamicClauseExecuted = false;

                SqlCommandBuilder nestedBuilder = new SqlCommandBuilder();

                foreach (SqlClause clause in _childs)
                {
                    if(clause.ToCommand(provider, nestedBuilder, parameters) && clause is DynamicClause)
                    {
                        anyNestedDynamicClauseExecuted = true;
                    }
                }

                //只有任何一个嵌套的动态语句被执行了才进行输出
                if (anyNestedDynamicClauseExecuted)
                {
                    builder.AppendCommandText(nestedBuilder.CommandText.ToString());
                    nestedBuilder.Params.ForEach(p => builder.AddCommandParameter(p));
                }

                return anyNestedDynamicClauseExecuted;
            }
            else
            {
                foreach (SqlClause clause in _childs)
                {
                    clause.ToCommand(provider, builder, parameters);
                }

                return true;                
            }
        }

        protected IList<SqlClause> ParseChilds()
        {
            /*
             * 一个动态子句的形式为：{? .. #ParamName# ..} 或者 {? .. $ParamName$ ..}.  或者 {? .. {? .. } {?..} }
             * 
             * 1.必须出现一个而且只能是一个参数子句，可以是命名参数或者值替换参数
             * 2.如果出现动态语句，那么不能出现参数语句
             */

            IList<SqlClause> clauses = SqlParser.ParseToClauses(Content);

            //如果找到动态语句，那么必须全部文本或动态语句
            DynamicClause clause = clauses.FirstOrDefault(c => c is DynamicClause) as DynamicClause;
            if (null != clause)
            {
                if (clauses.Any(c => c is ParameterClause))
                {
                    throw new FoxOneException("嵌套的动态语句{?..}和命名参数(#..#或$..$)不能同时出现在一个动态语句中 : " + RawText);
                }

                _foundNestedDynamicClause = true;
            }
            else if (!FindSingleParameterClause(clauses))
            {
                throw new FoxOneException("动态语句{?..}中必须包含命名参数或者嵌套的动态语句 : " + RawText);
            }

            return clauses;
        }

        private bool FindSingleParameterClause(IList<SqlClause> clauses)
        {
            ParameterClause paramClause = null;

            int count = 0;
            foreach (SqlClause clause in clauses)
            {
                if (clause is ParameterClause)
                {
                    count++;
                    paramClause = clause as ParameterClause;
                }

                if (count > 1)
                {
                    break;
                }
            }

            if (count == 1)
            {
                _paramClause = paramClause;
            }
            return count == 1;
        }
    }

    public class ActionClause : SqlClause
    {
        private readonly SqlAction _action;
        private readonly ISqlActionExecutor _executor;

        public ActionClause(string rawText, string action, string content)
        {
            RawText = rawText;
            _action = new SqlAction(action, content);
            _executor = DaoFactory.ActionExecutors.FirstOrDefault(o => o.Prefix.Equals(action, StringComparison.OrdinalIgnoreCase));

            if (null == _executor)
            {
                throw new InvalidOperationException(
                    string.Format("Unknow Sql Action '{0}' at : '{1}'", action, rawText));
            }
        }

        public SqlAction Action { get { return _action; } }

        public override bool ToCommand(IDaoProvider provider, SqlCommandBuilder builder, ISqlParameters parameters)
        {
            IDictionary<string, object> outParams = new Dictionary<string, object>();

            string sql = _executor.Execute(_action, parameters, outParams);

            if (!string.IsNullOrEmpty(sql))
            {
                parameters = outParams.Count == 0 ? parameters : new ArrayParameters(outParams, parameters);

                IList<SqlClause> clauses = SqlParser.ParseToClauses(sql);

                foreach (SqlClause clause in clauses)
                {
                    clause.ToCommand(provider, builder, parameters);
                }
            }

            return true;
        }
    }
}