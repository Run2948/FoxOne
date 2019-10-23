
using FoxOne.Data;
using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel;
namespace FoxOne.Business
{
    /// <summary>
    /// 数据字典数据源
    /// </summary>
    [DisplayName("数据字典数据源")]
    public class DictionaryDataSource : KeyValueDataSourceBase
    {
        [DisplayName("字典编号")]
        [FunctionDataSource(typeof(DictionaryCodeDataSource))]
        [FormField(ControlType = ControlType.DropDownList)]
        public string DictionaryCode { get; set; }
        public override IEnumerable<TreeNode> SelectItems()
        {
            var returnValue = new List<TreeNode>();
            var dic = DBContext<DataDictionary>.Instance.FirstOrDefault(o => o.Code.Equals(DictionaryCode, StringComparison.OrdinalIgnoreCase));
            if (dic != null)
            {
                if (!dic.Items.IsNullOrEmpty())
                {
                    foreach (var d in dic.Items)
                    {
                        returnValue.Add(new TreeNode() { Text = d.Name, Value = d.Code });
                    }
                }
                return returnValue;
            }
            throw new ArgumentOutOfRangeException("不存在DictionaryCode为{0}的字典".FormatTo(DictionaryCode));
        }
    }
}
