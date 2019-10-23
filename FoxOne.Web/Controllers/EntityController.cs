using FoxOne.Controls;
using FoxOne.Data;
using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Globalization;
using FoxOne.Business.Environment;
using System.Transactions;
using FoxOne.Business;

namespace FoxOne.Web.Controllers
{
    public class EntityController : BaseController
    {
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit()
        {
            IDictionary<string, object> data = Request.Form.ToDictionary();
            string key = Request.Form[NamingCenter.PARAM_KEY_NAME];
            string pageId = Request.QueryString[NamingCenter.PARAM_PAGE_ID];
            string ctrlId = Request.QueryString[NamingCenter.PARAM_CTRL_ID];
            string formViewMode = Request.Form[NamingCenter.PARAM_FORM_VIEW_MODE];
            var page = PageBuilder.BuildPage(pageId);
            if (page == null)
            {
                throw new FoxOneException("Page_Not_Found");
            }
            var form = page.FindControl(ctrlId) as Form;
            if (form == null)
            {
                throw new FoxOneException("Ctrl_Not_Found");
            }
            var ds = form.FormService as IFormService;
            int effectCount = 0;
            if (formViewMode.Equals(FormMode.Edit.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                effectCount = ds.Update(key, data);
            }
            else
            {
                effectCount = ds.Insert(data);
            }
            return Json(effectCount > 0);
        }

        [HttpPost]
        public JsonResult Delete()
        {
            string key = Request.Form[NamingCenter.PARAM_KEY_NAME];
            string pageId = Request.Form[NamingCenter.PARAM_PAGE_ID];
            string ctrlId = Request.Form[NamingCenter.PARAM_CTRL_ID];
            var page = PageBuilder.BuildPage(pageId);
            if (page == null)
            {
                throw new FoxOneException("Page_Not_Found");
            }
            var table = page.FindControl(ctrlId) as Table;
            if (table == null)
            {
                throw new FoxOneException("Ctrl_Not_Found");
            }
            var ds = table.DataSource as IFormService;
            if (key.IndexOf(",") > 0)
            {
                var keys = key.Split(',');
                int i = 0;
                foreach (var k in keys)
                {
                    i += ds.Delete(k);
                }
                return Json(i == keys.Length);
            }
            return Json(ds.Delete(key) > 0);
        }

        [HttpPost]
        public JsonResult GenerateCRUD()
        {
            string on = "on";
            string IsCRUD = Request.Form["IsCRUD"];
            string CRUDName = Request.Form["CRUDName"];
            string IsList = Request.Form["IsList"];
            string ListName = Request.Form["ListName"];
            string IsEdit = Request.Form["IsEdit"];
            string EditName = Request.Form["EditName"];
            string tableName = Request.Form["TableName"];
            string pageTitle = Request.Form["PageTitle"];
            var pageGenerator = new PageGenerator()
            {
                CRUDName = CRUDName,
                EditPageName = EditName,
                ListPageName = ListName,
                TableName = tableName,
                PageTitle = pageTitle
            };
            pageGenerator.AddCRUD();
            if (IsList == on)
            {
                pageGenerator.AddListPage();
            }
            if (IsEdit == on)
            {
                pageGenerator.AddEditPage();
            }
            return Json(true);
        }

        public FileResult ExportToExcel()
        {
            string ctrlId = Request[NamingCenter.PARAM_CTRL_ID];
            string pageId = Request[NamingCenter.PARAM_PAGE_ID];
            var Table = PageBuilder.BuildPage(pageId).FindControl(ctrlId) as Table;
            string templateName = Request.QueryString["TemplateFile"];
            int ingoreRow = Request.QueryString["ingoreRow"].ConvertTo<int>();
            string fileName = string.Format("{0}-{1}.xls", Table.Id, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            if (!Request.QueryString["fileName"].IsNullOrEmpty())
            {
                fileName = Request.QueryString["fileName"];
            }
            if (!fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                fileName = "{0}.xls".FormatTo(fileName);
            }
            int rowSpanColumnIndex = 0;
            if (!Request.QueryString["megerColumn"].IsNullOrEmpty())
            {
                rowSpanColumnIndex = Request.QueryString["megerColumn"].ConvertTo<int>();
            }
            int freezeColumn = 0;
            if (!Request.QueryString["freezeColumn"].IsNullOrEmpty())
            {
                freezeColumn = Request.QueryString["freezeColumn"].ConvertTo<int>();
            }
            string templatePath = templateName.IsNullOrEmpty() ? string.Empty : Server.MapPath("~/App_Config/ExcelTemplate/{0}".FormatTo(templateName));
            var workbook = new ExcelHelper().ExportToExcel(Table, freezeColumn, rowSpanColumnIndex, templatePath, ingoreRow);
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                return File(ms.GetBuffer(), "application/vnd.ms-excel", fileName);
            }
        }

        public JsonResult UserRole(string UserId, string RoleId, bool Add)
        {
            bool result = false;
            if(UserId.IsNullOrEmpty() || RoleId.IsNullOrEmpty())
            {
                throw new FoxOneException("UnValid UserId Or RoleId");
            }
            if (Add)
            {
                result = DBContext<UserRole>.Insert(new UserRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    RentId = 1,
                    RoleId = RoleId,
                    Status = DefaultStatus.Enabled.ToString(),
                    UserId = UserId
                });
            }
            else
            {
                var entity = DBContext<UserRole>.Instance.FirstOrDefault(o => o.UserId.Equals(UserId, StringComparison.OrdinalIgnoreCase) && o.RoleId.Equals(RoleId, StringComparison.OrdinalIgnoreCase));
                if(entity!=null)
                {
                    result = DBContext<UserRole>.Delete(entity);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
