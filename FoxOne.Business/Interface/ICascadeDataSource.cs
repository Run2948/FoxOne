using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FoxOne.Business
{
    public class TreeNode
    {
        [ScriptName("id")]
        public string Value { get; set; }

        [ScriptName("name")]
        public string Text { get; set; }

        [ScriptName("pId")]
        public string ParentId { get; set; }

        [ScriptName("icon")]
        public string Icon { get; set; }

        [ScriptName("isParent")]
        public bool IsParent { get; set; }

        [ScriptName("open")]
        public bool Open { get; set; }

        [ScriptName("url")]
        public string Url { get; set; }

        [ScriptName("checked")]
        public bool Checked { get; set; }

        [ScriptName("chkDisabled")]
        public bool CheckedDisabled { get; set; }
    }

    public interface ICascadeDataSource : IFieldConverter
    {
        IEnumerable<TreeNode> SelectItems();
    }
}
