using FoxOne.Business.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FoxOne.Core;
namespace FoxOne.Business
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {

                var result = new AjaxResultModel()
                {
                    LoginTimeOut = true,
                    ErrorMessage = "登录超时"
                };
                if (filterContext.HttpContext.Response.StatusCode == 403)
                {
                    filterContext.HttpContext.Response.StatusCode = 200;
                    result.NoAuthority = true;
                    result.ErrorMessage = "您没有访问资源[" + filterContext.RouteData.Values["controller"].ToString() + "/" + filterContext.RouteData.Values["action"].ToString() + "]的权限";
                }
                filterContext.Result = new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = result
                };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var returnValue = httpContext.User.Identity.IsAuthenticated;
            if (returnValue)
            {
                var request = httpContext.Request;
                if (!Sec.Provider.HasPermissionOfUrl(request.Path, request.Url.Query))
                {
                    httpContext.Response.StatusCode = 403;
                    if (httpContext.Request.IsAjaxRequest())
                    {
                        returnValue = false;
                    }
                    else
                    {
                        string redirect = "~/Home/Error/您没有访问该页面的权限";
                        httpContext.Response.Redirect(redirect);
                        httpContext.Response.End();
                    }
                }
            }
            return returnValue;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var attr = filterContext.ActionDescriptor.GetCustomAttributes(typeof(CustomUnAuthorizeAttribute), true);
            if (attr.IsNullOrEmpty())
            {
                base.OnAuthorization(filterContext);
            }
        }
    }

    public class CustomUnAuthorizeAttribute : Attribute
    {

    }
}
