using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
namespace FoxOne.Controls
{
    public class Link:FormControlBase
    { 
        protected override string TagName
        {
            get { return "a"; }
        }

        public Link()
        {
            CssClass = "btn btn-default";
        }

        public string Icon { get; set; }

        internal override void AddAttributes()
        {
            base.AddAttributes();
            Attributes["role"] = "button";
        }

        protected override string RenderInner()
        {
            return "<i class=\"Hui-iconfont {0}\"></i>".FormatTo(Icon);
        }
    }
}
