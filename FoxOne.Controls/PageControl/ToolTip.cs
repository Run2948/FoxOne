using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FoxOne.Core;

namespace FoxOne.Controls
{
    public class ToolTip:PageControlBase
    {
        public ToolTip()
        {
            CssClass = "alert";
            Attributes["role"] = "alert";
        }

        [DisplayName("标题")]
        public string Title { get; set; }

        [DisplayName("内容")]
        public string Content { get; set; }

        [DisplayName("提示类型")]
        public ToolTipType ToolTipType { get; set; }

        public override string Render()
        {
            CssClass+=" alert-"+ToolTipType.ToString().ToLower();
            return base.Render();
        }

        public override string RenderContent()
        {
            return "<strong>{0}</strong>{1}".FormatTo(Title,Content);
            //return base.RenderContent();
        }
    }

    public enum ToolTipType
    {
        Info,
        Warning,
        Danger,
        Success,
    }
}
