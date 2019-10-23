using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace FoxOne.Data.Sql
{
    public class SqlParser
    {

        //匹配#ParamName#或者$ParamName$
        private static readonly Regex ParameterPattern =
            new Regex(@"#(?<NamedParam>.+?)#|\$(?<ValueParam>.+?)\$", RegexOptions.Compiled);

        //匹配{?..}
        private static readonly Regex DynamicClausePattern =
            new Regex(@"\{\?(?<DynamicClause>(?>[^\{\}]+|\{\?(?<Clause>)|\}(?<-Clause>))*(?(Clause)(?!)))\}", RegexOptions.Compiled | RegexOptions.Singleline);

        //匹配@action{...}，其中action是可变的名字
        private static readonly Regex ActionClausePattern =
            new Regex(@"@(?<ActionName>[a-zA-Z0-9_-]+)\{(?<ActionText>.+?)\}", RegexOptions.Compiled | RegexOptions.Singleline);

        //匹配原生的命名参数@ParamName或者:ParamName，分别支持Sql Server和Oracle
        //todo 支持多种数据库时需要考虑
        private static readonly Regex DBNamedParamPattern =
            new Regex(@"[@:](?<Name>[a-zA-Z0-9_:-]+)");

        /// <summary>
        /// 把SQL文本解析为一个<see cref="T:FoxOne.Data.Sql.SqlStatement">SqlStatement</see>对象
        /// </summary>
        /// <param name="sql">sql文本</param>
        /// <param name="ignoreDbNamedParam">是否忽略掉sql中的数据库原生命名参数</param>
        /// <returns><see cref="T:FoxOne.Data.Sql.SqlStatement">SqlStatement</see>对象</returns>
        public static ISqlStatement Parse(string sql, bool ignoreDbNamedParam = false)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return new SqlStatement(sql, ParseToClauses(sql, ignoreDbNamedParam));
            }
            finally
            {
                sw.Stop();
                //Logger.Debug("Parsing Sql[Length={0}] -> used {1}ms", sql.Length, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ignoreDbNamedParam">是否忽略数据库本身的命名参数</param>
        /// <returns></returns>
        internal static IList<SqlClause> ParseToClauses(string sql, bool ignoreDbNamedParam = false)
        {
            sql = sql.Trim();

            IList<SqlClause> clauses = new List<SqlClause>();

            //解析@action{}语句
            IEnumerable<Match> matches = FindActionClauses(sql);

            //解析{? .. }语句
            matches = FindDynamicClauses(sql, matches);

            //解析#ParamName#和$ParamName$语句(不包含在{?..}语句中的参数)
            matches = FindDynamicParameters(sql, matches);

            //解析数据库本身的命名参数，如SqlServer是@ParamName，Oracle是:ParamName
            if (!ignoreDbNamedParam)
            {
                matches = FindProviderNamedParameters(sql, matches);
            }

            //如果没有发现任何参数和动态子句则整个sql作为一个TextClause);)
            if (matches.Count() == 0)
            {
                //Logger.Trace("Parser -> No any parameters or dynamic clauses found");
                clauses.Add(new TextClause(sql));
            }
            else
            {
                //按Index的升序排序
                matches = matches.OrderBy(match => match.Index);

                int lastIndex = 0;
                foreach (Match match in matches)
                {
                    if (match.Index > lastIndex)
                    {
                        string text = sql.Substring(lastIndex, match.Index - lastIndex);
                        clauses.Add(new TextClause(text));
                    }
                    lastIndex = match.Index + match.Length;

                    Group group;

                    if ((group = match.Groups["Name"]).Success)
                    {
                        clauses.Add(new DbNamedParameterClause(match.Value, group.Value));
                    }
                    else if ((group = match.Groups["NamedParam"]).Success)
                    {
                        clauses.Add(new NamedParameterClause(match.Value, group.Value));
                    }
                    else if ((group = match.Groups["ValueParam"]).Success)
                    {
                        //区分出like、in子句，在ValueParameter中特殊处理
                        if (clauses.Count > 0 && clauses.Last().RawText.IndexOf(" like ", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            clauses.Add(new ValueParameterClause(match.Value, group.Value, ValueParameterClauseUsage.Like));
                        }
                        else if (clauses.Count > 0 && (clauses.Last().RawText.IndexOf(" in ", StringComparison.OrdinalIgnoreCase) >= 0 || clauses.Last().RawText.IndexOf(" in(", StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            clauses.Add(new ValueParameterClause(match.Value, group.Value, ValueParameterClauseUsage.In));
                        }
                        else
                        {
                            clauses.Add(new ValueParameterClause(match.Value, group.Value, ValueParameterClauseUsage.Normal));
                        }
                    }
                    else if ((group = match.Groups["DynamicClause"]).Success)
                    {
                        clauses.Add(new DynamicClause(match.Value, group.Value));
                    }
                    else if ((group = match.Groups["ActionName"]).Success)
                    {
                        Group group1 = match.Groups["ActionText"];
                        clauses.Add(new ActionClause(match.Value, group.Value, group1.Value));
                    }
                }

                if (lastIndex < sql.Length)
                {
                    clauses.Add(new TextClause(sql.Substring(lastIndex)));
                }
            }

            //Logger.Trace("Parser -> Parsed to {0} sql clauses", clauses.Count);

            return clauses;
        }

        private static IEnumerable<Match> FindActionClauses(string sql)
        {
            IEnumerable<Match> actionClauseMatches = ActionClausePattern.Matches(sql).Cast<Match>();
            //Logger.Trace("Parser -> Found {0} matched action clauses", actionClauseMatches.Count());

            return actionClauseMatches;
        }

        private static IEnumerable<Match> FindDynamicClauses(string sql, IEnumerable<Match> matched)
        {
            IEnumerable<Match> dynamicClauseMatches = from dmatch in DynamicClausePattern.Matches(sql).Cast<Match>()
                                                      where !matched.Any(match => dmatch.Index >= match.Index &&
                                                                         dmatch.Index < match.Index + match.Length)
                                                      select dmatch;

            //Logger.Trace("Parser -> Found {0} matched dynamic clauses", dynamicClauseMatches.Count());

            return dynamicClauseMatches.Count() > 0 ? matched.Concat(dynamicClauseMatches) : matched;
        }

        private static IEnumerable<Match> FindDynamicParameters(string sql, IEnumerable<Match> matched)
        {
            IEnumerable<Match> parameterMatches = from pmatch in ParameterPattern.Matches(sql).Cast<Match>()
                                                  where !matched.Any(match => pmatch.Index >= match.Index &&
                                                                     pmatch.Index < match.Index + match.Length)
                                                  select pmatch;

            //Logger.Trace("Parser -> Found {0} matched parameters not in dynamic clauses", parameterMatches.Count());

            return parameterMatches.Count() > 0 ? matched.Concat(parameterMatches) : matched;
        }

        private static IEnumerable<Match> FindProviderNamedParameters(string sql, IEnumerable<Match> matched)
        {
            IEnumerable<Match> dbNamedParameterMatchs = from pmatch in DBNamedParamPattern.Matches(sql).Cast<Match>()
                                                        where !matched.Any(match => pmatch.Index >= match.Index &&
                                                                           pmatch.Index < match.Index + match.Length)
                                                              && (pmatch.Index > 0 && !"@".Equals(sql.Substring(pmatch.Index -1,1)))
                                                        select pmatch;

            //Logger.Trace("Parser -> Found {0} Named Parameters", dbNamedParameterMatchs.Count());

            if (dbNamedParameterMatchs.Count() > 0)
            {
                return matched.Concat(dbNamedParameterMatchs);
            }
            return matched;
        }
    }
}