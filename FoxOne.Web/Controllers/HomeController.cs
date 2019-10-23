using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using FoxOne.Data;
using FoxOne.Controls;
using FoxOne.Business;
using FoxOne.Core;
using FoxOne.Business.Security;
namespace FoxOne.Web.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetMenu()
        {
            var temp = Sec.Provider.GetAllUserPermission().Where(o => o.Type < PermissionType.Control && o.Status.Equals(DefaultStatus.Enabled.ToString(), StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.Rank);
            var result = new List<TreeNode>();
            temp.ForEach(o =>
            {
                result.Add(new TreeNode
                                {
                                    Value = o.Id,
                                    ParentId = o.ParentId,
                                    Text = o.Name,
                                    Url = o.Url,
                                    Icon = o.Icon
                                });
                if (o.Parent != null)
                {
                    if (result.Count(p => p.Value.Equals(o.Parent.Id, StringComparison.OrdinalIgnoreCase)) == 0)
                    {
                        result.Add(new TreeNode
                        {
                            Value = o.Parent.Id,
                            ParentId = "",
                            Text = o.Parent.Name,
                            Url = o.Parent.Url,
                            Icon = o.Parent.Icon
                        });
                    }
                }
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataBySqlId(string sqlId, string type)
        {
            if (DaoFactory.GetSqlSource().Find(sqlId) != null)
            {
                if (type.Equals("exec:", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(Dao.Get().ExecuteNonQuery(sqlId) > 0, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Dao.Get().QueryDictionaries(sqlId), JsonRequestBehavior.AllowGet);
                }
            }
            throw new FoxOneException("SqlId_Not_Found", sqlId);
        }

        public JsonResult GetWidgetData()
        {
            string pageId = Request[NamingCenter.PARAM_PAGE_ID];
            string ctrlId = Request[NamingCenter.PARAM_CTRL_ID];
            string widgetType = Request[NamingCenter.PARAM_WIDGET_TYPE];
            if (ctrlId.IsNullOrEmpty())
            {
                throw new FoxOneException("Parameter_Not_Found", NamingCenter.PARAM_CTRL_ID);
            }
            var ctrl = PageBuilder.BuildPage(pageId).FindControl(ctrlId);
            if (ctrl == null)
            {
                throw new FoxOneException("Ctrl_Not_Found", ctrlId);
            }
            if (widgetType.Equals("Chart", StringComparison.OrdinalIgnoreCase))
            {
                var chart = ctrl as NoPagerListControlBase;
                if (chart == null)
                {
                    throw new FoxOneException("Need_To_Be_NoPagerListControlBase", chart.Id);
                }
                if (chart.DataSource == null)
                {
                    throw new FoxOneException("Need_DataSource", chart.Id);
                }
                return Json(chart.GetData(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var tree = ctrl as Tree;
                if (tree == null)
                {
                    throw new FoxOneException("Need_To_Be_Tree", tree.Id);
                }
                if (tree.DataSource == null)
                {
                    throw new FoxOneException("Need_DataSource", tree.Id);
                }
                return Json(tree.DataSource.SelectItems(), JsonRequestBehavior.AllowGet);

            }
        }


        [CustomUnAuthorize]
        public ActionResult LogOn()
        {
            return View();
        }

        [CustomUnAuthorize]
        [HttpPost]
        public ActionResult LogOn(string userName, string password)
        {
            if (Sec.Provider.Authenticate(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, false);
                string returnUrl = Request.QueryString["ReturnUrl"];
                if (!returnUrl.IsNullOrEmpty())
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewData["ErrorMessage"] = ObjectHelper.GetObject<ILangProvider>().GetString("InValid_User_Or_Password");
                return View();
            }
        }

        [CustomUnAuthorize]
        public ActionResult Error(string id)
        {
            ViewData["ErrorMessage"] = id;
            return View();
        }

        public ActionResult LogOut()
        {
            Sec.Provider.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("LogOn");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangePassword(FormCollection form)
        {
            string password = form["OldPassword"];
            string newPassword = form["NewPassword"];
            string confirmPassword = form["ConfirmPassword"];

            if (!newPassword.Equals(confirmPassword, StringComparison.OrdinalIgnoreCase))
            {
                throw new FoxOneException("NewPassword_NotEqual_ConfirmPassword");
            }
            if (Sec.Provider.Authenticate(Sec.User.LoginId, password))
            {
                if (Sec.Provider.ResetPassword(Sec.User.LoginId, newPassword))
                {
                    return Json(true);
                }
            }
            else
            {
                throw new Exception("Invalid_Original_Password！");
            }
            return Json(false);
        }

        public ActionResult TableDemo()
        {
            return View();
        }

        public ActionResult ButtonDemo()
        {
            return View();
        }

        public ActionResult TreeForm()
        {
            return View();
        }
    }
}
