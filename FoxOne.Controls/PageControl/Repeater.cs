using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Web.Mvc;
using System.Web;
using System.Web.Script.Serialization;
using System.ComponentModel;
namespace FoxOne.Controls
{
    public class Repeater : ListControlBase
    {
        public Repeater()
        {
            AllowPaging = true;
            PagerPosition = PagerPosition.Bottom;
            Pager = new Pager();
            ModelName = "model";
            SubContainerTag = FoxOne.Controls.SubContainerTag.none;
        }

        [DisplayName("单项显示html模板")]
        [HtmlEncode]
        [FormField(ControlType = ControlType.TextArea)]
        public string ItemTemplate { get; set; }

        [DisplayName("子容器标签")]
        public SubContainerTag SubContainerTag { get; set; }

        [DisplayName("子容器CssClass")]
        public string SubContainerCssClass { get; set; }

        [FormField(ControlType = ControlType.TextArea)]
        [DisplayName("无数据时显示html模板")]
        public string EmptyTemplate { get; set; }

        [FormField(ControlType = ControlType.TextArea)]
        [DisplayName("分隔区域html模板")]
        public string SeperatorTemplate { get; set; }

        public string GroupBy { get; set; }

        [DisplayName("实体类型")]
        public string ModelName { get; set; }

        [DisplayName("排序字段")]
        public string SortExpression { get; set; }

        public override string RenderContent()
        {
            int recordCount = 0;
            IEnumerable<IDictionary<string, object>> entities = GetData();
            if (entities.IsNullOrEmpty())
            {
                return EmptyTemplate;
            }
            StringBuilder result = new StringBuilder();
            if (AllowPaging)
            {
                Pager.TargetId = Id;
                if (PagerPosition == PagerPosition.Top || PagerPosition == PagerPosition.Both)
                {
                    result.AppendLine(Pager.Render());
                }
            }
            Pager.RecordCount = recordCount;
            var content = new List<string>();
            entities.ForEach((entity) =>
            {
                StringTemplate query = new StringTemplate(ItemTemplate);
                query.SetAttribute(entity);
                content.Add(query.ToString());
            });
            result.AppendLine(string.Join(SeperatorTemplate, content.ToArray()));
            if (AllowPaging)
            {
                if (PagerPosition == PagerPosition.Bottom || PagerPosition == PagerPosition.Both)
                {
                    result.AppendLine(Pager.Render());
                }
            }
            if (SubContainerTag!= SubContainerTag.none)
            {
                var subContainer = new TagBuilder(SubContainerTag.ToString());
                if (!SubContainerCssClass.IsNullOrEmpty())
                {
                    subContainer.AddCssClass(SubContainerCssClass);
                }
                subContainer.InnerHtml = result.ToString();
                return subContainer.ToString();
            }
            return result.ToString();
        }
    }

    public enum SubContainerTag
    {
        none,
        div,
        ul,
        ol
    }
}
