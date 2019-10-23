using System.Collections.Generic;

namespace FoxOne.Data
{
    public interface ISqlCommand
    {
        /// <summary>
        /// 带参数的sql语句，已经按照数据库类型正确处理参数表达方式
        /// </summary>
        string CommandText { get; }

        //sql中参数的参数名和对应的值
        IList<KeyValuePair<string, object>> Parameters { get; }
    }
}