
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public class Layout 
    {
        private Page _page;

        public Layout(Page page, LayoutEntity layout)
        {
            _page = page;
            ID = layout.Id;
            ExtFiles = layout.ExtFiles;
            Html = layout.Html;
            StartUpScript = layout.StartUpScript;
            Style = layout.Style;

        }
        public string ID
        {
            get;
            set;
        }


        public Page Page { get { return _page; } }

        public IList<ExternalFileEntity> ExtFiles { get; set; }

        public string StartUpScript { get; set; }

        public string ScriptBlock { get; set; }

        public string Style { get; set; }

        public string Html { get; set; }

        public void Render()
        {
            foreach (var extCssFile in ExtFiles.Where(o => o.Type == "CSS").OrderBy(o => o.Rank))
            {
                Page.RegisterExtCssFile(extCssFile.Name, extCssFile.Path);
            }
            foreach (var extJsFile in ExtFiles.Where(o => o.Type == "JS").OrderBy(o => o.Rank))
            {
                Page.RegisterExtJsFile(extJsFile.Name, extJsFile.Path);
            }
            Page.AddMetaData("contentType", "http-equiv", "Content-Type");
            Page.AddMetaData("contentType", "content", "text/html; charset=utf-8");
            Page.AddMetaData("chartSet", "charset", "utf-8");
            Page.AddMetaData("viewportMeta", "name", "viewport");
            Page.AddMetaData("viewportMeta", "content", "width=device-width, initial-scale=1.0");
            Page.RegisterStartUpScript("layoutStartUpScript", StartUpScript);
            Page.RegisterScriptBlock("layoutScriptBlock", ScriptBlock);
            Page.RegisterStyleContent("layoutStyle", Style);
        }
    }
}
