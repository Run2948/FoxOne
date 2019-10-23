using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FoxOne.Business
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Logger.Error(filterContext.Exception.Message, filterContext.Exception);
            if (!(filterContext.Exception is FoxOneException))
            {
                //预料之外的异常，记录日志
                
            }
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                JsonResult json = new JsonResult();
                AjaxResultModel result = new AjaxResultModel()
                {
                    Result = false,
                    //ErrorMessage = "出错的地址是：" + filterContext.RouteData.Values["controller"].ToString() + "/" + filterContext.RouteData.Values["action"].ToString() + "\n异常信息为：" + filterContext.Exception.Message
                    ErrorMessage = filterContext.Exception.Message
                };
                json.Data = result;
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = json;
            }
            else
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                string message = string.Empty;
                if (filterContext.Exception is PageNotFoundException)
                {
                    filterContext.HttpContext.Response.StatusCode = 404;
                    message = "页面不存在";
                }
                else if (filterContext.Exception is UnAuthorizedException)
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    message = "您没有访问该页面的权限";
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 500;
                    message = filterContext.Exception.Message;
                }
                filterContext.Result = new RedirectResult("/Home/Error/" + message);
            }
        }
    }
}
