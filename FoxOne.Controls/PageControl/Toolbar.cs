using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.ComponentModel;
using System.Web.Script.Serialization;
using FoxOne.Data.Mapping;
using FoxOne.Business.Security;
namespace FoxOne.Controls
{
    /// <summary>
    /// 工具栏组件
    /// </summary>
    [DisplayName("工具栏组件")]
    public class Toolbar : PageControlBase,ITargetId,IAuthorityComponent
    {
        public Toolbar()
        {
            Buttons = new List<Button>();
            CssClass = "toolbar";
        }

        /// <summary>
        /// 工具栏按钮
        /// </summary>
        [DisplayName("工具栏按钮")]
        public IList<Button> Buttons { get; set; }


        public void SetTarget(IList<IControl> components)
        {

        }

        public override string Render()
        {
            if (Buttons.Count(o=>o.Visiable)==0)
            {
                return string.Empty;
            }
            return base.Render();
        }

        public override string RenderContent()
        {
            if (Buttons.IsNullOrEmpty())
            {
                throw new FoxOneException("Buttons不能为空");
            }
            StringBuilder sb = new StringBuilder();
            foreach (var button in Buttons.OrderBy(o=>o.Rank))
            {
                sb.AppendLine(button.Render());
            }
            return sb.ToString();
        }

        public void Authority(IDictionary<string, UISecurityBehaviour> behaviour)
        {
            foreach (var button in Buttons)
            {
                if (behaviour.Keys.Contains(button.Id))
                {
                    button.Visiable = !behaviour[button.Id].IsInvisible;
                }
            }
        }

        /// <summary>
        /// 目标控件ID
        /// </summary>
        [DisplayName("目标控件ID")]
        public string TargetControlId
        {
            get;
            set;
        }
    }
}
