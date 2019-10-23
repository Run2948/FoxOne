using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Business;
using System.Web.Script.Serialization;
namespace FoxOne.Controls
{
    [DisplayName("树型控件")]
    public class Tree : PageControlBase, ICascadeDataSourceControl
    {
        public Tree()
        {
            TreeCssClass = "ztree";
            ShowCheck = false;
        }

        public bool ShowCheck { get; set; }

        public string TreeCssClass { get; set; }

        public override string Render()
        {
            Attributes["showcheck"] = ShowCheck.ToString().ToLower();
            return base.Render();
        }

        public override string RenderContent()
        {
            if (DataSource == null)
            {
                throw new FoxOneException("需要为Tree设置数据源");
            }
            return "<ul id=\"{0}-ul\" class=\"{1}\"></ul>".FormatTo(Id, TreeCssClass);
        }

        [DisplayName("数据源")]
        public ICascadeDataSource DataSource
        {
            get;
            set;
        }
    }

}
