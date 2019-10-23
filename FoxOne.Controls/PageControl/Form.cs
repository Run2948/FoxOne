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
using System.Threading;
using FoxOne.Business.Security;
using FoxOne.Data.Mapping;
namespace FoxOne.Controls
{

    /// <summary>
    /// 表单组件
    /// </summary>
    [DisplayName("表单组件")]
    public class Form : PageControlBase, ICloneable, IAuthorityComponent
    {
        public Form()
        {
            Fields = new List<FormControlBase>();
            Buttons = new List<Button>();
            FormMode = FormMode.Insert;
            FormCssClass = "form form-horizontal";
            AppendQueryString = true;
            AutoHeight = true;
        }

        /// <summary>
        /// 表单模式
        /// </summary>
        [DisplayName("表单模式")]
        public FormMode FormMode { get; set; }


        public object Data { get; set; }

        [DisplayName("附加URL参数为隐藏域")]
        public bool AppendQueryString { get; set; }


        /// <summary>
        /// 表单字段
        /// </summary>
        [DisplayName("表单字段")]
        public IList<FormControlBase> Fields { get; set; }

        /// <summary>
        /// 表单按钮
        /// </summary>
        [DisplayName("表单按钮")]
        public IList<Button> Buttons { get; set; }

        public string FormEnctype
        {
            get;
            set;
        }


        /// <summary>
        /// 表单提交地址
        /// </summary>
        [DisplayName("表单提交地址")]
        public string PostUrl { get; set; }

        /// <summary>
        /// 表单服务类
        /// </summary>
        [DisplayName("表单服务类")]
        public IFormService FormService { get; set; }

        public string FormTemplate { get; set; }

        public string FormCssClass { get; set; }

        public string Key { get; set; }

        public override string Render()
        {
            if (Fields.Count(o => o.Visiable) == 0)
            {
                return string.Empty;
            }
            return base.Render();
        }

        public override string RenderContent()
        {
            if (Fields.Count == 0)
            {
                throw new FoxOneException("Fields不能为空");
            }
            var request = HttpContext.Current.Request;

            if (FormService != null)
            {
                if (Key.IsNullOrEmpty())
                {
                    Key = request[NamingCenter.PARAM_KEY_NAME];
                }

                //当FORM指定的服务类不为空，且当前的请求中带有KEY的URL参数，
                //则尝试去获取表单数据，如果不为空，则为编辑状态
                if (!Key.IsNullOrEmpty())
                {
                    var tempData = FormService.Get(Key);
                    if (!tempData.IsNullOrEmpty())
                    {
                        FormMode = FormMode.Edit;
                        Data = tempData;
                    }
                }
            }
            IDictionary<string, object> formData = null;
            if (Data != null)
            {
                formData = Data as Dictionary<string, object>;
                if (formData.IsNullOrEmpty())
                {
                    formData = Data.ToDictionary();
                }
            }
            StringBuilder result = new StringBuilder();
            ///隐藏域和按钮属于特殊群体放在表单的指定位置
            StringBuilder hiddenResult = new StringBuilder();
            StringBuilder buttonResult = new StringBuilder();
            if (!Buttons.IsNullOrEmpty())
            {
                foreach (var button in Buttons)
                {
                    buttonResult.AppendLine(button.Render());
                }
            }
            var formModeHidden = new HiddenField() { Id = NamingCenter.PARAM_FORM_VIEW_MODE, Value = request[NamingCenter.PARAM_FORM_VIEW_MODE].IsNullOrEmpty() ? FormMode.ToString() : request[NamingCenter.PARAM_FORM_VIEW_MODE], Name = NamingCenter.PARAM_FORM_VIEW_MODE };
            hiddenResult.Append(formModeHidden.Render());
            var formTemplate = TemplateGenerator.GetDefaultFormTemplate();
            var renderQuerystringFinished = false;
            if (AppendQueryString && !renderQuerystringFinished)
            {
                foreach (var r in request.QueryString.AllKeys)
                {
                    if (!r.Equals(NamingCenter.PARAM_FORM_VIEW_MODE, StringComparison.OrdinalIgnoreCase) && Fields.Count(o => o.Id.Equals(r, StringComparison.CurrentCultureIgnoreCase)) == 0)
                    {
                        hiddenResult.AppendLine(new HiddenField() { Id = r, Name = r, Value = request.QueryString[r] }.Render());
                    }
                }
                renderQuerystringFinished = true;
            }
            foreach (var field in Fields.OrderBy(o => o.Rank))
            {
                if (field.Name.IsNullOrEmpty())
                {
                    field.Name = field.Id;
                }
                if (!formData.IsNullOrEmpty())
                {
                    if (formData.Keys.Contains(field.Id))
                    {
                        field.Value = (formData[field.Id] == null) ? "" : formData[field.Id].ToString();
                    }
                }
                else
                {
                    if (!request.QueryString[field.Id].IsNullOrEmpty())
                    {
                        //如果URL参数中含有与当前控件ID一致的键值，则以URL参数的值赋初始值
                        field.Value = request.QueryString[field.Id];
                    }
                    else
                    {
                        /// 当字段的值存在的时候从环境变量将值格式化
                        if (field.Value != null)
                        {
                            field.Value = Env.Parse(field.Value.ToString());
                        }
                    }
                }
                if (field is HiddenField)
                {
                    hiddenResult.AppendLine(field.Render());
                }
                else
                {
                    if (field.ContainerTemplate.IsNullOrEmpty())
                    {
                        field.ContainerTemplate = TemplateGenerator.GetFormFieldTemplate();
                    }
                    result.AppendLine(field.Render());
                }
            }
            return formTemplate.FormatTo(PostUrl, FormCssClass, hiddenResult.ToString(), result.ToString(), buttonResult.ToString());
        }

        public void Authority(IDictionary<string, UISecurityBehaviour> behaviour)
        {
            foreach (var field in Fields)
            {
                if (behaviour.Keys.Contains(field.Id))
                {
                    field.Visiable = !behaviour[field.Id].IsInvisible;
                }
            }
        }

        public override object Clone()
        {
            var result = base.Clone() as Form;
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
    }
}
