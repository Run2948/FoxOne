using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Business;
using System.Web.Mvc;
using System.ComponentModel;
using FoxOne.Business.Security;
namespace FoxOne.Controls
{
    public abstract class PageControlBase : ComponentBase, IAttributeAccessor
    {
        public PageControlBase()
        {
            Visiable = true;
            Attributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// 是否自动计算高度
        /// </summary>
        [DisplayName("是否自动计算高度")]
        public bool AutoHeight { get; set; }

        /// <summary>
        /// 自动计算高度偏移
        /// </summary>
        [DisplayName("自动计算高度偏移")]
        public int AutoHeightOffset { get; set; }

        public IDictionary<string, string> Attributes { get; private set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        [DisplayName("扩展属性")]
        public string AttributeString
        {
            get;
            set;
        }

        public override string Render()
        {
            if (Visiable)
            {
                TagBuilder div = new TagBuilder("div");
                div.Attributes["id"] = Id;
                div.Attributes["widget"] = this.GetType().Name;
                div.Attributes["PageId"] = PageId;
                if (AutoHeight)
                {
                    div.Attributes["data-autoHeight"] = "true";
                    div.Attributes["data-autoOffset"] = AutoHeightOffset.ToString();
                }
                if (this is ITargetId)
                {
                    Attributes["data-target"] = (this as ITargetId).TargetControlId;
                }
                if (!AttributeString.IsNullOrEmpty())
                {
                    string[] kvs = AttributeString.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var kv in kvs)
                    {
                        string[] keyValue = kv.Split(':');
                        Attributes[keyValue[0]] = keyValue[1];
                    }
                }
                foreach (var key in Attributes.Keys)
                {
                    div.Attributes[key] = Attributes[key];
                }
                if (!CssClass.IsNullOrEmpty())
                {
                    div.AddCssClass(CssClass);
                }
                try
                {
                    div.InnerHtml = RenderContent();
                }
                catch (Exception ex)
                {
                    var errorTipSpan = new TagBuilder("div");
                    errorTipSpan.Attributes["style"] = "color:red;padding:20px;border:1px solid red;";
                    string stackTraceInfo = ex.StackTrace.Replace("在", "<br />在");
                    errorTipSpan.InnerHtml = ex.Message + stackTraceInfo;
                    div.InnerHtml = errorTipSpan.ToString();
                }
                return div.ToString();
            }
            return string.Empty;
        }

        public virtual string RenderContent()
        {
            return string.Empty;
        }

        public override object Clone()
        {
            var result = this.MemberwiseClone() as PageControlBase;
            result.Attributes = new Dictionary<string, string>();
            foreach(var attr in Attributes)
            {
                result.Attributes.Add(attr.Key, attr.Value);
            }
            return result;
        }
    }
}
