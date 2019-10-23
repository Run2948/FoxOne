using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.ComponentModel;
using System.Web.Mvc;
namespace FoxOne.Controls
{

    /// <summary>
    /// 日期控件
    /// </summary>
    [DisplayName("日期控件")]
    public class DatePicker : TextBox
    {
        public DatePicker()
            : base()
        {
            ShowPreNextButton = true;
            ChangeTiggerSearch = false;
        }
        public string DateTimeFormat { get; set; }

        public string Maximum { get; set; }

        public string MaxDateControl { get; set; }

        public string MinDateControl { get; set; }

        public string Minimum { get; set; }

        public bool ShowWeek { get; set; }

        public string OnDatePicked { get; set; }

        public bool GreaterThanToday { get; set; }

        /// <summary>
        /// 是否显示 上一天，下一天 按钮
        /// </summary>
        public bool ShowPreNextButton { get; set; }

        private string StringProperty(string str)
        {
            return "'{0}'".FormatTo(str);
        }

        internal override void AddAttributes()
        {
            base.AddAttributes();
            var innerAttr = new Dictionary<string, string>();
            if (DateTimeFormat.IsNullOrEmpty())
            {
                DateTimeFormat = "yyyy-MM-dd";
            }
            if(!Value.IsNullOrEmpty())
            {
                DateTime dt;
                if(DateTime.TryParse(Value,out dt))
                {
                    Value = dt.ToString(DateTimeFormat);
                }
            }
            innerAttr.Add("dateFmt", StringProperty(DateTimeFormat));
            if (ChangeTiggerSearch)
            {
                innerAttr.Add("onpicked", "function(){$(this).closest('[searchForm]').submit();}");
            }
            else
            {
                if (!OnDatePicked.IsNullOrEmpty())
                {
                    innerAttr.Add("onpicked", OnDatePicked);
                }
            }
            if (!Maximum.IsNullOrEmpty())
            {
                innerAttr.Add("maxDate", StringProperty(Maximum));
            }
            if (!String.IsNullOrEmpty(Minimum))
            {
                innerAttr.Add("minDate", StringProperty(Minimum));
            }
            if (GreaterThanToday)
            {
                innerAttr.Add("minDate", StringProperty(DateTime.Now.ToString()));
            }
            if (!string.IsNullOrEmpty(MinDateControl))
            {
                innerAttr.Add("minDate", "\'#F{$dp.$D(\\\'" + MinDateControl + "\\\');}\'");
            }
            if (!string.IsNullOrEmpty(MaxDateControl))
            {
                innerAttr.Add("maxDate", "\'#F{$dp.$D(\\\'" + MaxDateControl + "\\\');}\'");
            }
            if (ShowWeek)
            {
                innerAttr.Add("isShowWeek", "true");
            }
            StringBuilder innerAttrStr = new StringBuilder();
            innerAttr.ForEach((dict) =>
            {
                innerAttrStr.AppendFormat(",{0}:{1}", dict.Key, dict.Value);
            });
            Attributes["onclick"] = "WdatePicker({{{0}}});".FormatTo(innerAttrStr.ToString().Substring(1));
            Attributes["value"] = Value;
        }

        protected override string RenderAfter()
        {
            var result = string.Empty;
            if (ShowPreNextButton)
            {
                var div = new TagBuilder("div");
                div.AddCssClass("btn-group");
                var btn = new TagBuilder("button");
                btn.AddCssClass("btn btn-default btn-sm");
                btn.SetInnerText("上一天");
                btn.Attributes.Add("onclick", "foxOne.changeDatePickerDate('{0}',-1)".FormatTo(Id));
                div.InnerHtml = btn.ToString();
                btn = new TagBuilder("button");
                btn.AddCssClass("btn btn-default btn-sm");
                btn.Attributes.Add("onclick", "foxOne.changeDatePickerDate('{0}',1)".FormatTo(Id));
                btn.SetInnerText("下一天");
                div.InnerHtml += btn.ToString();
                result = "\n" + div.ToString();
            }
            return result;
        }
    }
}
