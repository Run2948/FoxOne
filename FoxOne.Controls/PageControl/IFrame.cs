using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Web.Mvc;
namespace FoxOne.Controls
{
    public class IFrame : PageControlBase
    {
        public IFrame()
        {
            Height = "99%";
            Width = "100%";
        }
        public string Src { get; set; }

        public string Height { get; set; }

        public string Width { get; set; }

        public bool Scrolling { get; set; }

        public override string RenderContent()
        {
            var result = new TagBuilder("iframe");
            result.Attributes["src"] = Src;
            result.Attributes["width"] = Width;
            result.Attributes["height"] = Height;
            result.Attributes["frameborder"] = "0";
            result.Attributes["scrolling"] = Scrolling ? "auto" : "no";
            return result.ToString();
        }
    }
}
