using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Business;
using System.ComponentModel;
using System.Web.Mvc;
using FoxOne.Business.Environment;

namespace FoxOne.Controls
{

    /// <summary>
    /// 日期区间控件
    /// </summary>
    [DisplayName("日期区间控件")]
    public class TimeRangePicker : FormControlBase
    {
        protected override string TagName
        {
            get { return "div"; }
        }

        public string DateTimeFormat { get; set; }

        public string Maximum { get; set; }

        public string Minimum { get; set; }

        public bool ShowWeek { get; set; }

        public bool GreaterThanToday { get; set; }

        public string DefaultStartValue { get; set; }

        public string DefaultEndValue { get; set; }

        public override string Render()
        {
            if (Visiable)
            {
                if (!this.Value.IsNullOrEmpty())
                {
                    string[] timeRangeArr = this.Value.Split(',');
                    DefaultStartValue = timeRangeArr[0];
                    if (timeRangeArr.Length > 1)
                    {
                        DefaultEndValue = timeRangeArr[1];
                    }
                }
                if(Name.IsNullOrEmpty())
                {
                    Name = Id;
                }
                if(!DefaultStartValue.IsNullOrEmpty())
                {
                    DefaultStartValue = Env.Parse(DefaultStartValue);
                }
                if (!DefaultEndValue.IsNullOrEmpty())
                {
                    DefaultEndValue = Env.Parse(DefaultEndValue);
                }
                DatePicker sdt = new DatePicker() { Value = DefaultStartValue, ShowPreNextButton = false, CssClass = this.CssClass, DateTimeFormat = this.DateTimeFormat, GreaterThanToday = this.GreaterThanToday, Maximum = this.Maximum, Minimum = this.Minimum, ShowWeek = this.ShowWeek, Id = NamingCenter.GetTimeRangeDatePickerStartId(this.Name), Name = NamingCenter.GetTimeRangeDatePickerStartId(this.Name), MaxDateControl = NamingCenter.GetTimeRangeDatePickerEndId(this.Name) };
                DatePicker edt = new DatePicker() { Value = DefaultEndValue, ShowPreNextButton = false, CssClass = this.CssClass, DateTimeFormat = this.DateTimeFormat, GreaterThanToday = this.GreaterThanToday, Maximum = this.Maximum, Minimum = this.Minimum, ShowWeek = this.ShowWeek, Id = NamingCenter.GetTimeRangeDatePickerEndId(this.Name), Name = NamingCenter.GetTimeRangeDatePickerEndId(this.Name), MinDateControl = NamingCenter.GetTimeRangeDatePickerStartId(this.Name) };
                string result = "<label>从&nbsp;</label>{0}<label>&nbsp;到&nbsp;</label>{1}".FormatTo(sdt.Render(), edt.Render());
                return ContainerTemplate.FormatTo(Id, Label, result, Description);
            }
            return string.Empty;
        }
    }
}
