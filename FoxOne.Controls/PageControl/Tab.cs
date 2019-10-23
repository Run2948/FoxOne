using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Web.Mvc;
using System.ComponentModel;
using System.Web.Script.Serialization;
using FoxOne.Business.Security;
namespace FoxOne.Controls
{
    /// <summary>
    /// 选项卡组件
    /// </summary>
    [DisplayName("选项卡组件")]
    public class Tab : PageControlBase, IKeyValueDataSourceControl,IAuthorityComponent
    {
        public Tab()
        {
            TabItems = new List<TabItem>();
            TabItemContainerCss = "tab-box";
        }

        /// <summary>
        /// 初始显示项索引
        /// </summary>
        [DisplayName("初始显示项索引")]
        public int InitIndex { get; set; }

        /// <summary>
        /// 选项卡容器CSS
        /// </summary>
        [DisplayName("选项卡容器CSS")]
        public string TabItemContainerCss { get; set; }

        /// <summary>
        /// 选项卡项
        /// </summary>
        [DisplayName("选项卡项")]
        public IList<TabItem> TabItems { get; set; }

        public override string Render()
        {
            if (DataSource != null)
            {
                var data = DataSource.SelectItems();
                foreach (var d in data)
                {
                    TabItems.Add(new TabItem()
                    {
                        Id = d.Value,
                        TabName = d.Text,
                        Visiable = true
                    });
                }
            }
            if (TabItems.Count(o => o.Visiable) == 0)
            {
                return string.Empty;
            }
            return base.Render();
        }

        public override string RenderContent()
        {
            if (TabItems.IsNullOrEmpty())
            {
                throw new FoxOneException("TabItems不能为空");
            }
            var tabBox = new TagBuilder("div");
            tabBox.AddCssClass(TabItemContainerCss);
            var content = string.Empty;
            var span = string.Empty;
            foreach (var item in TabItems.OrderBy(o => o.Rank))
            {
                tabBox.InnerHtml += item.RenderTab();
                content += item.Render();
            }
            return tabBox.ToString() + content;
        }

        public void Authority(IDictionary<string, UISecurityBehaviour> behaviour)
        {
            foreach (var item in TabItems)
            {
                if (behaviour.Keys.Contains(item.Id))
                {
                    item.Visiable = !behaviour[item.Id].IsInvisible;
                }
            }
        }

        public IKeyValueDataSource DataSource
        {
            get;
            set;
        }


    }

    /// <summary>
    /// 选项卡项
    /// </summary>
    [DisplayName("选项卡项")]
    public class TabItem : ComponentBase
    {
        public TabItem()
        {
            Content = new List<PageControlBase>();
            CssClass = "tab-item";
            Visiable = true;
        }

        /// <summary>
        /// 是否延迟加载
        /// </summary>
        [DisplayName("是否延迟加载")]
        public bool LazyLoad { get; set; }

        /// <summary>
        /// 项名称
        /// </summary>
        [DisplayName("项名称")]
        public string TabName { get; set; }

        /// <summary>
        /// 项内容
        /// </summary>
        [DisplayName("项内容")]
        public IList<PageControlBase> Content { get; set; }

        /// <summary>
        /// 项图标
        /// </summary>
        [DisplayName("项图标")]
        public string Icon { get; set; }

        public string RenderTab()
        {
            if (Visiable)
            {
                var tabItem = new TagBuilder("span");
                tabItem.AddCssClass(CssClass);
                tabItem.Attributes["key"] = Id;
                if (!Icon.IsNullOrEmpty())
                {
                    var img = new TagBuilder("img");
                    img.Attributes["src"] = Icon;
                    img.Attributes["alt"] = TabName;
                    tabItem.InnerHtml = img.ToString();
                }
                tabItem.SetInnerText(TabName);
                return tabItem.ToString();
            }
            return string.Empty;
        }

        public override string Render()
        {
            if (Visiable)
            {

                if (!Content.IsNullOrEmpty())
                {
                    //当内容中有iframe，则只支持一个
                    if (Content[0] is IFrame)
                    {
                        var c = Content[0] as IFrame;
                        c.Attributes["src1"] = c.Src;
                        c.Attributes["tabItem"] = Id;
                        c.Attributes["id"] = Id;
                        c.Src = "";
                        return c.Render();
                    }
                    else
                    {
                        var div = new TagBuilder("div");
                        div.Attributes["tabItem"] = Id;
                        div.Attributes["id"] = Id;
                        if(LazyLoad)
                        {
                            div.Attributes["contentId"] = string.Join("|", Content.Select(o => o.Id).ToArray());
                        }
                        else
                        {
                            foreach (var c in Content)
                            {
                                div.InnerHtml += c.Render();
                            }
                        }
                        return div.ToString();
                    }
                }
            }
            return string.Empty;
        }
    }
}
