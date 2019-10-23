using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FoxOne.Core;
using FoxOne.Controls;
using FoxOne.Business.Environment;
namespace FoxOne.Web.Controllers
{
    public class PageController : BaseController
    {
        public string Index(string pageId, string ctrlId)
        {
            var page = PageBuilder.BuildPage(pageId);
            if (page == null)
            {
                throw new PageNotFoundException();
            }
            page.PreRender();
            if (!ctrlId.IsNullOrEmpty())
            {
                var ctrl = page.FindControl(ctrlId) as IComponent;
                if (ctrl == null)
                {
                    throw new FoxOneException("Ctrl_Not_Found", ctrlId);
                }
                return ctrl.Render();
            }
            return page.Render();
        }

        public string PreView()
        {
            string pageId = Request.QueryString[NamingCenter.PARAM_PAGE_ID];
            var page = PageBuilder.BuildPage(pageId);
            page.RegisterStartUpScript("Design", "foxOne.design();");
            page.PreRender();
            return page.Render();
        }
    }
}
