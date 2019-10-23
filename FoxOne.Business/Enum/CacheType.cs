using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    /// <summary>
    /// 框架缓存类型
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// 页面配置
        /// </summary>
        [Description("页面配置")]
        PAGE_CONFIG,

        /// <summary>
        /// 数据库表
        /// </summary>
        [Description("数据库表")]
        DATA_TABLE,

        /// <summary>
        /// 实体配置
        /// </summary>
        [Description("实体配置")]
        ENTITY_CONFIG
    }
}
