using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Web.Mvc;
using System.ComponentModel;
namespace FoxOne.Business
{
    [DisplayName("分隔字符数据源")]
    public class StringSplitDataSource : KeyValueDataSourceBase
    {
        [Validator("required")]
        [Description("可以用逗号，竖线，分号等分隔")]
        public string SplitString { get; set; }

        public override IEnumerable<TreeNode> SelectItems()
        {
            var result = new List<TreeNode>();
            if (!SplitString.IsNullOrEmpty())
            {
                var items = SplitString.Split(new char[] { ',', '|', ';' });
                items.ForEach(o =>
                {
                    result.Add(new TreeNode()
                    {
                        Text = o,
                        Value = o
                    });
                });
            }
            return result;
        }
    }
}
