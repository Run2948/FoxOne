using FoxOne.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    /// <summary>
    /// SqlId树型数据源
    /// </summary>
    [DisplayName("SqlId树型数据源")]
    public class SqlIdCascadeDataSource:KeyValueDataSourceBase,ICascadeDataSource
    {
        [FunctionDataSource(typeof(AllSqlIdDataSource))]
        [FormField(ControlType = ControlType.DropDownList)]
        public string SqlId { get; set; }

        public override IEnumerable<TreeNode> SelectItems()
        {
            return Dao.Get().QueryEntities<TreeNode>(SqlId);
        }
    }
}
