using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FoxOne.Core;
namespace FoxOne.Controls
{
    public class Image:PageControlBase
    {
        public string ImgSrc { get; set; }

        public override string RenderContent()
        {
            var img = new TagBuilder("img");
            if (!ImgSrc.IsNullOrEmpty())
            {
                img.Attributes["src"] = ImgSrc;
            }
            return img.ToString();
        }
    }
}
