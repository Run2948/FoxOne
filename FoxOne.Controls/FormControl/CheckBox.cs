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
    /// 复选框
    /// </summary>
    [DisplayName("复选框")] 
    public class CheckBox : FormControlBase
    {
        public CheckBox()
            : base()
        {
            Disabled = false;
            CssClass = "checkbox";
        }

        protected override bool SelfClosing
        {
            get
            {
                return true;
            }
        }

        protected override string TagName
        {
            get { return "input"; }
        }


        public bool Checked { get; set; }

        public string Text { get; set; }

        public bool Disabled { get; set; }

        internal override void AddAttributes()
        {
            base.AddAttributes();
            Attributes["type"] = "checkbox";
            Attributes["value"] = Value;
            if (Checked)
            {
                Attributes["checked"] = "checked";
            }
            if (Disabled)
            {
                Attributes["disabled"] = "disabled";
            }
        }

        public override string Render()
        {
            string b = base.Render();
            return "<label class=\"checkbox-radio\">{0}{1}</label>".FormatTo(b, Text);
        }
    }
}
