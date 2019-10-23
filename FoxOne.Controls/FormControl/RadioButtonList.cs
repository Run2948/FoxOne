using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Web.Mvc;
using System.Threading;
using System.ComponentModel;
using System.Web.Script.Serialization;
namespace FoxOne.Controls
{

    /// <summary>
    /// 单选框集合
    /// </summary>
    [DisplayName("单选框集合")]
    public class RadioButtonList : CheckBoxList
    {
        public RadioButtonList()
        {
            CssClass = "";
        }

        protected override string RenderInner()
        {
            StringBuilder content = new StringBuilder();
            var items = GetData();
            foreach (var item in items)
            {
                var radio = new RadioButton() { Value = item.Value, Checked = item.Checked, Id = Id + "_" + item.Value, Name = Name, Text = item.Text };
                content.AppendLine(radio.Render());
            }
            return content.ToString();
        }
    }
}
