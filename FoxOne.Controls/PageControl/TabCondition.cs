using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FoxOne.Core;
namespace FoxOne.Controls
{
    public class TabCondition : PageControlBase, ITargetId, IKeyValueDataSourceControl
    {
        public TabCondition()
        {
            CssClass = "tab-box";
        }

        public string SearchField
        {
            get;
            set;
        }

        public void SetTarget(IList<IControl> components)
        {
            if (!Value.IsNullOrEmpty() && !SearchField.IsNullOrEmpty())
            {
                var formData = new FoxOneDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                formData[SearchField] = Value;
                foreach (var c in components)
                {
                    if (c is IListDataSourceControl)
                    {
                        var ds = (c as IListDataSourceControl).DataSource;
                        if (ds != null)
                        {
                            if (ds.Parameter == null)
                            {
                                ds.Parameter = formData;
                            }
                            else
                            {
                                var dict = ds.Parameter as Dictionary<string, object>;
                                foreach (var f in formData.Keys)
                                {
                                    dict[f] = formData[f];
                                }
                            }
                        }
                    }
                }
            }
        }

        [DisplayName("默认值")]
        public string Value
        {
            get;
            set;
        }

        public IKeyValueDataSource DataSource
        {
            get;
            set;
        }

        public override string Render()
        {
            Attributes["data-target"] = TargetControlId;
            Attributes["data-field"] = SearchField;
            return base.Render();
        }

        public override string RenderContent()
        {
            if (DataSource == null)
            {
                throw new ArgumentNullException("DataSource");
            }
            var tabItems = DataSource.SelectItems();
            string tabBox = string.Empty;
            foreach (var item in tabItems)
            {
                tabBox += RenderTab(item);
            }
            return tabBox;
        }

        public string RenderTab(TreeNode item)
        {
            var tabItem = new TagBuilder("span");
            tabItem.AddCssClass("tab-item");
            tabItem.Attributes["key"] = item.Value;
            tabItem.InnerHtml = item.Text;
            if (item.Value.Equals(Value, StringComparison.CurrentCultureIgnoreCase))
            {
                tabItem.AddCssClass("tab-item-selected");
            }
            return tabItem.ToString();
        }

        public string TargetControlId
        {
            get;
            set;
        }
    }
}
