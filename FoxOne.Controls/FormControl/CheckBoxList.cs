using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FoxOne.Core;
using System.Threading;
using System.ComponentModel;
using System.Web.Script.Serialization;
namespace FoxOne.Controls
{

    /// <summary>
    /// 复选框集合
    /// </summary>
    [DisplayName("复选框集合")]
    public class CheckBoxList : KeyValueControlBase
    {
        public CheckBoxList()
        {
            CssClass = "";
            AppendEmptyOption = false;
        }

        protected override string TagName
        {
            get { return "div"; }
        }

        internal override void AddAttributes()
        {
            this.CssClass = "";
            base.AddAttributes();
            Attributes.Remove("name");
        }

        protected override string RenderInner()
        {
            StringBuilder content = new StringBuilder();
            var items = GetData();
            items.ForEach(item =>
            {
                var checkbox = new CheckBox() { Value = item.Value, Checked = item.Checked, Id = Id + "_" + item.Value, Name = Name, Text = item.Text };
                content.AppendLine(checkbox.Render());
            });
            return content.ToString();
        }
    }
}
