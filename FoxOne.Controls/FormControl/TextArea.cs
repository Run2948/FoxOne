using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.ComponentModel;
namespace FoxOne.Controls
{

    /// <summary>
    /// 多行文本框
    /// </summary>
    [DisplayName("多行文本框")]
    public class TextArea : FormControlBase
    {
        public TextArea()
            : base()
        {
        }
        public string Height { get; set; }
        protected override string TagName
        {
            get
            {
                return "textarea";
            }
        }

        protected override string RenderInner()
        {
            return Value;
        }

        public override string Render()
        {
            if(!Height.IsNullOrWhiteSpace())
            {
                if (Attributes.ContainsKey("style"))
                    Attributes["style"] += "height:{0}px".FormatTo(Height);
                else
                    Attributes["style"] = "height:{0}px".FormatTo(Height);
            }
            return base.Render();
        }
    }
}
