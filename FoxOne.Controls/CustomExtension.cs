using FoxOne.Business;
using FoxOne.Business.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FoxOne.Controls
{
    public static class CustomExtension
    {
        public static MvcHtmlString CustomControl(this HtmlHelper html, IComponent component)
        {
            if (component != null)
            {
                var behaviour = Sec.Provider.GetUISecurityBehaviours(HttpContext.Current.Request.FilePath, HttpContext.Current.Request.Url.Query);

                if (component is IAuthorityComponent)
                {
                    (component as IAuthorityComponent).Authority(behaviour);
                }
                return MvcHtmlString.Create(component.Render());
            }
            return MvcHtmlString.Create("");
        }

        public static MvcHtmlString CustomControl(this HtmlHelper html, string pageName, string componentId)
        {
            var page = PageBuilder.BuildPage(pageName);
            if (page != null)
            {
                var component = page.FindControl(componentId) as IComponent;
                if (component != null)
                {
                    var behaviour = Sec.Provider.GetUISecurityBehaviours(HttpContext.Current.Request.FilePath, HttpContext.Current.Request.Url.Query);
                    if (component is IAuthorityComponent)
                    {
                        (component as IAuthorityComponent).Authority(behaviour);
                    }
                    return MvcHtmlString.Create(component.Render());
                }
            }
            return MvcHtmlString.Create("");
        }
    }
}
