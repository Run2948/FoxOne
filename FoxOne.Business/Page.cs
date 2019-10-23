using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FoxOne.Core;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using FoxOne.Business.Security;
namespace FoxOne.Business
{
    public class Page
    {
        private IList<TagBuilder> links;
        private IList<TagBuilder> scripts;
        private IDictionary<string, TagBuilder> metas;
        private TagBuilder script;
        private TagBuilder style;
        private IList<string> keys;
        private StringBuilder startUpScript;
        private PageEntity PageEntity;
        public Page(PageEntity pageEntity)
        {
            PageEntity = pageEntity;
            Id = pageEntity.Id;
            Title = pageEntity.Title;
            ExtFiles = pageEntity.ExtFiles;
            StartUpScript = pageEntity.StartUpScript;
            ScriptBlock = pageEntity.ScriptBlock;
            Style = pageEntity.Style;
            CssClass = pageEntity.CssClass;
            Children = new List<IComponent>();
            Controls = new List<IControl>();
            var componentEntities = pageEntity.Components;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new ComponentConverter() });
            foreach (var component in componentEntities)
            {
                var type = TypeHelper.GetType(component.Type);
                if (!component.JsonContent.IsNullOrEmpty())
                {
                    var instance = serializer.Deserialize(component.JsonContent, type);
                    var control = instance as IControl;
                    control.ParentId = component.ParentId;
                    control.PageId = component.PageId;
                    control.TargetId = component.TargetId;
                    Controls.Add(control);
                }
            }
            foreach (var e in Controls)
            {
                if (e.ParentId.Equals(pageEntity.Id))
                {
                    Children.Add(e as IComponent);
                    GetChildren(e);
                }
            }
        }

        private void GetChildren(IControl e)
        {
            var children = Controls.Where(o => o.ParentId.Equals(e.Id));
            if (children.IsNullOrEmpty()) return;
            var fastType = FastType.Get(e.GetType());
            foreach (var ee in children)
            {
                var gettter = fastType.GetGetter(ee.TargetId);
                if (gettter.Type.IsGenericType)
                {
                    var instance = gettter.GetValue(e);
                    if (instance == null)
                    {
                        var t = typeof(List<>);
                        var type = gettter.Type.GetGenericArguments()[0];
                        t = t.MakeGenericType(type);
                        instance = Activator.CreateInstance(t);
                        gettter.SetValue(e, instance);
                    }
                    var add = instance.GetType().GetMethod("Add");
                    add.Invoke(instance, new object[] { ee });
                }
                else
                {
                    gettter.SetValue(e, ee);
                }
                GetChildren(ee);
            }
        }

        public void Reset()
        {
            links = new List<TagBuilder>();
            scripts = new List<TagBuilder>();
            metas = new Dictionary<string, TagBuilder>();
            script = new TagBuilder("script");
            script.Attributes.Add("type", "text/javascript");
            style = new TagBuilder("style");
            style.Attributes.Add("type", "text/css");
            keys = new List<string>();
            startUpScript = new StringBuilder();
        }
        public string Id
        {
            get;
            set;
        }
        public string Title { get; set; }

        public string StartUpScript { get; set; }

        public string ScriptBlock { get; set; }

        public string Style { get; set; }

        public string CssClass { get; set; }

        public HttpRequest Request
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        public IList<ExternalFileEntity> ExtFiles { get; set; }

        public IList<IComponent> Children { get; set; }

        public IList<IControl> Controls
        {
            get;
            set;
        }

        public IControl FindControl(string controlId)
        {
            return Controls.FirstOrDefault(o => o.Id.Equals(controlId, StringComparison.OrdinalIgnoreCase));
        }

        private Layout _layout;
        public Layout Layout
        {
            get
            {
                return _layout ?? (_layout = new Layout(this, PageEntity.Layout));
            }
        }

        public IPageService Service { get; set; }

        /// <summary>
        /// 注册外置CSS文件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cssFile"></param>
        public void RegisterExtCssFile(string key, string cssFile)
        {
            key = "CssFile-{0}".FormatTo(key);
            if (!keys.Contains(key))
            {
                keys.Add(key);
                var linkTag = new TagBuilder("link");
                linkTag.Attributes.Add("href", cssFile);
                linkTag.Attributes.Add("rel", "stylesheet");
                links.Add(linkTag);
            }
        }

        /// <summary>
        /// 注册外置JS文件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsFile"></param>
        public void RegisterExtJsFile(string key, string jsFile)
        {
            key = "JsFile-{0}".FormatTo(key);
            if (!keys.Contains(key))
            {
                keys.Add(key);
                var scriptTag = new TagBuilder("script");
                scriptTag.Attributes.Add("src", jsFile);
                scripts.Add(scriptTag);
            }
        }

        /// <summary>
        /// 注册启动脚本
        /// </summary>
        /// <param name="key"></param>
        /// <param name="script"></param>
        public void RegisterStartUpScript(string key, string script)
        {
            key = "StartUpScript-{0}".FormatTo(key);
            if (!keys.Contains(key))
            {
                keys.Add(key);
                startUpScript.AppendLine(script);
            }
        }

        /// <summary>
        /// 注册脚本语句块
        /// </summary>
        /// <param name="key"></param>
        /// <param name="scriptContent"></param>
        public void RegisterScriptBlock(string key, string scriptContent)
        {
            key = "ScriptContent-{0}".FormatTo(key);
            if (!keys.Contains(key))
            {
                keys.Add(key);
                script.AppendInnerHtml(scriptContent);
            }
        }

        /// <summary>
        /// 注册样式语句块
        /// </summary>
        /// <param name="key"></param>
        /// <param name="styleContent"></param>
        public void RegisterStyleContent(string key, string styleContent)
        {
            key = "CssContent-{0}".FormatTo(key);
            if (!keys.Contains(key))
            {
                keys.Add(key);
                style.AppendInnerHtml(styleContent);
            }
        }

        /// <summary>
        /// 注册meta标签
        /// </summary>
        /// <param name="metaKey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddMetaData(string metaKey, string key, string value)
        {
            TagBuilder meta;
            if (metas.Keys.Contains(metaKey))
            {
                meta = metas[metaKey];
            }
            else
            {
                meta = new TagBuilder("meta");
                metas.Add(metaKey, meta);
            }
            meta.Attributes[key] = value;
        }

        /// <summary>
        /// 预呈现
        /// </summary>
        public void PreRender()
        {
            Layout.Render();
            foreach (var extCssFile in ExtFiles.Where(o => o.Type == "CSS").OrderBy(o => o.Rank))
            {
                this.RegisterExtCssFile(extCssFile.Name, extCssFile.Path);
            }
            foreach (var extJsFile in ExtFiles.Where(o => o.Type == "JS").OrderBy(o => o.Rank))
            {
                this.RegisterExtJsFile(extJsFile.Name, extJsFile.Path);
            }
            this.RegisterStartUpScript("PageStartUpScript", StartUpScript);
            this.RegisterScriptBlock("PageScriptBlock", ScriptBlock);
            this.RegisterStyleContent("PageStyle", Style);
            var behaviour = Sec.Provider.GetUISecurityBehaviours(Request.FilePath, Request.Url.Query);
            if (!Children.IsNullOrEmpty())
            {
                foreach (var c in Controls)
                {
                    if (c is IAuthorityComponent)
                    {
                        (c as IAuthorityComponent).Authority(behaviour);
                    }
                    if (c is ITargetId)
                    {
                        var targetControl = c as ITargetId;
                        if (!targetControl.TargetControlId.IsNullOrEmpty())
                        {
                            var tempControl = new List<IControl>();
                            foreach (var id in targetControl.TargetControlId.Split(','))
                            {
                                tempControl.Add(FindControl(id));
                            }
                            targetControl.SetTarget(tempControl);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 呈现HTML
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            string documentType = "<!DOCTYPE html>\n";
            TagBuilder html = new TagBuilder("html");
            html.AppendInnerHtml(RenderHeader());
            html.AppendInnerHtml(RenderBody());
            return documentType + html.ToString();
        }

        private string RenderBody()
        {
            TagBuilder body = new TagBuilder("body");
            body.AddCssClass(CssClass);
            body.AppendInnerHtml(RenderControls());
            body.AppendInnerHtml(System.Web.Optimization.Scripts.Render("~/script/common.js").ToHtmlString());
            body.AppendInnerHtml(System.Web.Optimization.Scripts.Render("~/script/widget.js").ToHtmlString());
            foreach (var jsFile in scripts)
            {
                body.AppendInnerHtml(jsFile.ToString());
            }
            script.AppendInnerHtml("$(function(){\n" + startUpScript.ToString() + "\n});");
            body.AppendInnerHtml(script.ToString());
            return body.ToString();
        }

        private string RenderHeader()
        {
            TagBuilder header = new TagBuilder("head");
            TagBuilder title = new TagBuilder("title");
            title.SetInnerText(Title);
            foreach (var key in metas.Keys)
            {
                header.AppendInnerHtml(metas[key].ToString(TagRenderMode.SelfClosing));
            }
            header.AppendInnerHtml(title.ToString());
            header.AppendInnerHtml(System.Web.Optimization.Styles.Render("~/style/common.css").ToHtmlString());
            foreach (var cssFile in links)
            {
                header.AppendInnerHtml(cssFile.ToString(TagRenderMode.SelfClosing));
            }
            if (!style.InnerHtml.IsNullOrEmpty())
            {
                header.InnerHtml += style.ToString();
            }
            return header.ToString();
        }

        private string RenderControls()
        {
            StringBuilder result = new StringBuilder();
            var layoutTemplate = HttpUtility.HtmlDecode(Layout.Html);
            var matchs = Regex.Split(layoutTemplate, @"\$(?<Variable>.+?)\$", RegexOptions.Compiled);
            foreach (var match in matchs)
            {
                var childrens = Children.Where(o => o.TargetId.Equals(match));
                if (childrens.Count() > 0)
                {
                    foreach (var c in childrens.OrderBy(o => o.Rank))
                    {
                        result.AppendLine(c.Render());
                    }
                }
                else
                {
                    result.Append(match);
                }
            }
            return result.ToString();
        }
    }


    public static class TagBuilderExtension
    {
        public static void AppendInnerHtml(this TagBuilder tag, string html)
        {
            tag.InnerHtml += "\n{0}".FormatTo(html);
        }
    }

    public static class PageBuilder
    {
        public static Page BuildPage(string id)
        {
            var pageEntity = DBContext<PageEntity>.Instance.FirstOrDefault(o => o.Id.Equals(id.ToString(), StringComparison.CurrentCultureIgnoreCase));
            if (pageEntity == null)
            {
                throw new PageNotFoundException();
            }
            pageEntity.Components = DBContext<ComponentEntity>.Instance.Where(o => o.PageId.Equals(pageEntity.Id, StringComparison.CurrentCultureIgnoreCase)).ToList();
            pageEntity.Layout = DBContext<LayoutEntity>.Instance.Get(pageEntity.LayoutId);
            var pageFiles = DBContext<PageLayoutFileEntity>.Instance.Where(o => o.PageOrLayoutId.Equals(pageEntity.Id, StringComparison.CurrentCultureIgnoreCase)).Select(o => o.FileId);
            pageEntity.ExtFiles = DBContext<ExternalFileEntity>.Instance.Where(o => pageFiles.Contains(o.Id, StringComparer.OrdinalIgnoreCase)).ToList();
            var layoutFiles = DBContext<PageLayoutFileEntity>.Instance.Where(o => o.PageOrLayoutId.Equals(pageEntity.LayoutId, StringComparison.OrdinalIgnoreCase)).Select(o => o.FileId);
            pageEntity.Layout.ExtFiles = DBContext<ExternalFileEntity>.Instance.Where(o => layoutFiles.Contains(o.Id, StringComparer.OrdinalIgnoreCase)).ToList();
            Page result = null;
            if (!SysConfig.SystemStatus.Equals("Develop", StringComparison.CurrentCultureIgnoreCase))
            {
                string key = NamingCenter.GetCacheKey(CacheType.PAGE_CONFIG, pageEntity.Id);
                result = CacheHelper.GetFromCache<Page>(key, () =>
                {
                    return new FoxOne.Business.Page(pageEntity);
                });
            }
            else
            {
                result = new FoxOne.Business.Page(pageEntity);
            }
            result.Reset();
            return result;
        }
    }
}
