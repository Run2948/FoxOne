using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FoxOne.Data
{
    public interface ISqlSource
    {
        Dictionary<string, ISqlStatement> Sqls { get; }
        /// <summary>
        /// 判断Sql语句的唯一Key是否有效
        /// </summary>
        /// <remarks>
        /// 默认规则：key不能包含空格、逗号、括号
        /// </remarks>
        bool IsValidKey(string key);

        /// <summary>
        /// 添加一个SQL语句
        /// </summary>
        /// <param name="key"></param>
        /// <param name="statement"></param>
        /// <param name="overwite"></param>
        /// <returns></returns>
        bool Add(string key, ISqlStatement statement, bool overwite = true);

        /// <summary>
        /// 根据唯一Key查找返回一个经过Parse的SQL语句
        /// </summary>
        /// <param name="key">通过IsValidKey检查的唯一标识一条SQL语句的字符串</param>
        /// <returns>没有对应的语句返回null</returns>
        ISqlStatement Find(string key);

        /// <summary>
        /// 根据唯一Key和定义的DaoProvider名称（表示不同的数据库类型）查找返回一个经过Parse的SQL语句
        /// </summary>
        /// <param name="key">通过IsValidKey检查的唯一标识一条SQL语句的字符串</param>
        /// <param name="daoProviderName">对应的DaoProvider的名称，如SqlServer</param>
        /// <returns>没有对应的语句返回null</returns>
        ISqlStatement Find(string key, string daoProviderName);

        /// <summary>
        /// 加载单个assembly里的sql配置信息到内存中
        /// </summary>
        /// <param name="assembly">assembly对象</param>
        /// <param name="overrideSql">是否覆盖已有Key的sql</param>
        void LoadSqlsFromAssembly(Assembly assembly, bool overrideSql);
    }
}