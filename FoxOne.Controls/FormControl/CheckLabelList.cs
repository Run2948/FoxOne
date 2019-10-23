using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FoxOne.Core;
using System.ComponentModel;

namespace FoxOne.Controls
{
    public class CheckLabelList : KeyValueControlBase
    { 
        public CheckLabelList()
            : base()
        {
            AppendEmptyOption = true;
            EmptyOptionText = "全部";
            ChangeTiggerSearch = true;
        }

        protected override string TagName
        {
            get { return "div"; }
        }

        protected override string RenderInner()
        {
            StringBuilder result = new StringBuilder();
            var items = GetData();
            string selected = string.Empty;
            int i = 0;
            foreach (var item in items)
            {
                selected = item.Checked ? "active" : "";
                if (i == 0)
                {
                    result.AppendLine("<a href=\"#\" class=\"btn btn-default {3}\" value=\"{1}\">{0}<input name=\"{2}\" value=\"{4}\" type=\"hidden\"></a>".FormatTo(item.Text, item.Value, Name, selected, Value));
                }
                else
                {
                    result.AppendLine("<a href=\"#\" class=\"btn btn-default {2} \" value=\"{1}\">{0}</a>".FormatTo(item.Text, item.Value, selected));
                }
                i++;
            }
            return result.ToString();
        }
    }
}
