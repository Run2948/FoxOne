using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Web.Mvc;
using System.ComponentModel;
using System.Web.Script.Serialization;
namespace FoxOne.Controls
{

    /// <summary>
    /// 下拉框
    /// </summary>
    [DisplayName("下拉框")]
    public class DropDownList : KeyValueControlBase
    {

        public DropDownList():base()
        {
            AppendEmptyOption = true;
            EmptyOptionText = "==请选择==";
        }

        protected override string TagName
        {
            get { return "select"; }
        }

        protected override string RenderInner()
        {
            StringBuilder content = new StringBuilder();
            var items = GetData();
            string optionTemplate = "<option value=\"{0}\" {1} >{2}</option>";
            foreach(var item in items)
            {
                content.AppendLine(optionTemplate.FormatTo(item.Value, item.Checked ? "selected=\"selected\"" : "", item.Text));
            }
            return  content.ToString();
        }
    }
}
