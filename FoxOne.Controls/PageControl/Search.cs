using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Web;
using FoxOne.Business.Environment;
using System.ComponentModel;
using System.Web.Script.Serialization;
using FoxOne.Data.Mapping;
namespace FoxOne.Controls
{
    /// <summary>
    /// 搜索组件
    /// </summary>
    [DisplayName("搜索组件")]
    public class Search : PageControlBase, ITargetId
    {
        public Search()
        {
            Fields = new List<FormControlBase>();
            Buttons = new List<Button>();
            CssClass = "search pt10 pl10";
            FormCssClass = "form-inline";
            FormLayoutType = FoxOne.Controls.FormLayoutType.Vertical;
        }

        public void SetTarget(IList<IControl> components)
        {
            if (!Fields.IsNullOrEmpty())
            {
                var formData = new FoxOneDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                string key = string.Empty;
                var request = HttpContext.Current.Request;
                foreach (var f in Fields)
                {
                    if (f is TimeRangePicker)
                    {
                        //日期范围控件特殊处理
                        var dt = f as TimeRangePicker;
                        if (!dt.DefaultStartValue.IsNullOrEmpty())
                        {
                            formData.Add(NamingCenter.GetTimeRangeDatePickerStartId(dt.Name), DateTime.Parse(Env.Parse(dt.DefaultStartValue)).ToString(dt.DateTimeFormat));
                        }
                        if (!dt.DefaultEndValue.IsNullOrEmpty())
                        {
                            formData.Add(NamingCenter.GetTimeRangeDatePickerEndId(dt.Name), DateTime.Parse(Env.Parse(dt.DefaultEndValue)).ToString(dt.DateTimeFormat));
                        }
                    }
                    else if (f is DatePicker)
                    {
                        if (!f.Value.IsNullOrEmpty())
                        {
                            key = f.Name.IsNullOrEmpty() ? f.Id : f.Name;
                            var d = f as DatePicker;
                            formData.Add(key, DateTime.Parse(Env.Parse(f.Value)).ToString(d.DateTimeFormat));
                        }
                    }
                    else
                    {
                        if (!f.Value.IsNullOrEmpty())
                        {
                            key = f.Name.IsNullOrEmpty() ? f.Id : f.Name;
                            if (!request.Form.AllKeys.Contains(key) && !request.QueryString.AllKeys.Contains(key))
                            {
                                formData.Add(key, Env.Parse(f.Value));
                            }
                        }
                    }
                }
                foreach (var c in components)
                {
                    if (c is IListDataSourceControl)
                    {
                        var ds = (c as IListDataSourceControl).DataSource;
                        if (ds != null)
                        {
                            if (ds.Parameter == null)
                            {
                                ds.Parameter = formData;
                            }
                            else
                            {
                                var dict = ds.Parameter as Dictionary<string, object>;
                                foreach (var f in formData.Keys)
                                {
                                    dict[f] = formData[f];
                                }
                            }
                        }
                    }
                }
            }
        }

        public FormLayoutType FormLayoutType { get; set; }

        /// <summary>
        /// 搜索字段
        /// </summary>
        [DisplayName("搜索字段")]
        public IList<FormControlBase> Fields { get; set; }

        /// <summary>
        /// 搜索按钮
        /// </summary>
        [DisplayName("搜索按钮")]
        public IList<Button> Buttons { get; set; }

        public string SearchFormTemplate { get; set; }

        public override string Render()
        {
            if (Fields.Count(o => o.Visiable) == 0)
            {
                return string.Empty;
            }
            if (FormLayoutType == FoxOne.Controls.FormLayoutType.Horizontal)
            {
                CssClass = "search";
                FormCssClass = "form-list";
            }
            else
            {
                if(CssClass.Equals("search", StringComparison.OrdinalIgnoreCase))
                {
                    CssClass = "search pt10 pl10";
                }
            }
            return base.Render();
        }

        public override string RenderContent()
        {
            if (Fields.Count == 0)
            {
                throw new FoxOneException("Fields不能为空");
            }
            StringBuilder result = new StringBuilder();
            SearchFormTemplate = TemplateGenerator.GetSearchFormTemplate();
            string searchFieldTemplate = TemplateGenerator.GetFormFieldTemplate();
            string buttonTemplate = string.Empty;
            if (!Buttons.IsNullOrEmpty())
            {
                buttonTemplate = "<div class=\"search-control\"><div class=\"form-group\">{0}</div></div>";
                StringBuilder buttonResult = new StringBuilder();
                foreach (var button in Buttons.OrderBy(o => o.Rank))
                {
                    buttonResult.AppendLine(button.Render());
                }
                buttonTemplate = buttonTemplate.FormatTo(buttonResult.ToString());
            }
            foreach (var field in Fields.OrderBy(o => o.Rank))
            {
                if (field is HiddenField) continue;
                if (field.ContainerTemplate.IsNullOrEmpty())
                {
                    field.ContainerTemplate = searchFieldTemplate;
                }
                if (field.Name.IsNullOrEmpty())
                {
                    field.Name = field.Id;
                }
                if (!HttpContext.Current.Request.QueryString[field.Id].IsNullOrEmpty())
                {
                    //如果URL参数中含有与当前查询控件ID一致的键值，则以URL参数的值赋初始值，并且改当前查询控件为禁用状态
                    field.Value = HttpContext.Current.Request.QueryString[field.Id];
                }
                else
                {
                    field.Enable = true;
                    if (field.Value != null)
                    {
                        field.Value = Env.Parse(field.Value.ToString());
                    }
                }
                result.AppendLine(field.Render());
            }
            return SearchFormTemplate.FormatTo(FormCssClass, result.ToString(), buttonTemplate);
        }

        public string FormCssClass
        {
            get;
            set;
        }

        public override object Clone()
        {
            var result = base.Clone() as Search;
            result.Buttons = new List<Button>();
            foreach (var button in Buttons)
            {
                result.Buttons.Add(button.Clone() as Button);
            }
            result.Fields = new List<FormControlBase>();
            foreach (var field in Fields)
            {
                result.Fields.Add(field.Clone() as FormControlBase);
            }
            return result;
        }

        public string TargetControlId
        {
            get;
            set;
        }
    }

    public enum FormLayoutType
    {
        Vertical,
        Horizontal
    }
}
