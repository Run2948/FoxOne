using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.ComponentModel;
namespace FoxOne.Controls
{

    /// <summary>
    /// 文本框
    /// </summary>
    [DisplayName("文本框")]
    public class TextBox : FormControlBase
    {
        public TextBox()
            : base()
        {

        }
        protected override bool SelfClosing
        {
            get
            {
                return true;
            }
        }

        public string AutoCompleteSqlId { get; set; }

        protected override string TagName
        {
            get
            {
                return "input";
            }
        }
        public TextMode TextMode { get; set; }

        internal override void AddAttributes()
        {
            base.AddAttributes();
            Attributes["type"] = TextMode.ToString().ToLower();
            Attributes["value"] = Value;
            if (!AutoCompleteSqlId.IsNullOrEmpty())
            {
                Attributes["autoCompleteSqlId"] = AutoCompleteSqlId;
            }
        }

    }

    public enum TextMode
    {
        Text,
        Password,
    }
}
