using FoxOne.Business;
using FoxOne.Business.Security;
using FoxOne.Controls;
using FoxOne.Core;
using FoxOne.Data;
using FoxOne.Data.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Serialization;
namespace FoxOne.Web.Controllers
{
    public class InitController : BaseController
    {
        IList<Type> types = TypeHelper.GetAllImpl<IAutoCreateTable>();

        [CustomUnAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [CustomUnAuthorize]
        public ActionResult HomeIndex()
        {
            Sec.Provider.ResetPassword("liuhf", "123");
            FormsAuthentication.SetAuthCookie("liuhf", false);
            return RedirectToAction("Index", "Home");
        }

        [CustomUnAuthorize]
        [HttpPost]
        public JsonResult CreateTable()
        {
            types.ForEach(o =>
            {
                Dao.Get().CreateTable(o);
            });
            TableMapper.ClearTableMapping();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [CustomUnAuthorize]
        [HttpPost]
        public JsonResult ClearTable()
        {
            types.ForEach(o =>
            {
                Dao.Get().BatchDelete(o, null);
            });
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [CustomUnAuthorize]
        [HttpPost]
        public JsonResult InitData()
        {
            var dirInfo = new DirectoryInfo(Server.MapPath("~/InitData"));
            var files = dirInfo.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var type = typeof(List<>);
                if (type == null) continue;
                type = type.MakeGenericType(TypeHelper.GetType(file.Name.Replace(file.Extension, "")));
                var serializer = new XmlSerializer(type);
                var result = serializer.Deserialize(file.OpenRead()) as IEnumerable;
                foreach (var item in result)
                {
                    Dao.Get().Insert(item);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [CustomUnAuthorize]
        public JsonResult Out()
        {
            var dirInfo = Server.MapPath("~");
            if (!dirInfo.EndsWith("\\"))
            {
                dirInfo += "\\";
            }
            dirInfo = "{0}InitData\\".FormatTo(dirInfo);
            if (!Directory.Exists(dirInfo))
            {
                Directory.CreateDirectory(dirInfo);
            }
            var allTypes = TypeHelper.GetAllSubType<EntityBase>();
            string fileName = string.Empty;
            foreach (var type in allTypes)
            {
                var t = typeof(List<>);
                fileName = type.FullName;
                t = t.MakeGenericType(type);
                var instance = Activator.CreateInstance(t);
                var serializer = new XmlSerializer(t);
                Dao.Get().Select(type).ForEach(o =>
                {
                    t.InvokeMember("Add", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, instance, new object[] { o });
                });
                var stream = System.IO.File.Create(dirInfo + fileName + ".xml");
                serializer.Serialize(stream, instance);
                stream.Close();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
