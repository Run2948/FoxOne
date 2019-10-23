using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Globalization;
using FoxOne.Business;
using FoxOne.Core;
using FoxOne.Controls;
using System.ComponentModel;
namespace FoxOne.Web.Controllers
{
    [CustomAuthorize]
    [CustomHandleError]
    public class BaseController : Controller
    {
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            if (behavior == JsonRequestBehavior.DenyGet
                && string.Equals(this.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                return new JsonResult();
            }
            AjaxResultModel result = new AjaxResultModel()
            {
                Data = data
            };
            return new CustomJsonResult() { Data = result, ContentType = contentType, ContentEncoding = contentEncoding, JsonRequestBehavior = behavior };
        }
    }


    public class CustomJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] { new DateTimeConverter() });
                if (MaxJsonLength.HasValue)
                {
                    serializer.MaxJsonLength = MaxJsonLength.Value;
                }
                if (RecursionLimit.HasValue)
                {
                    serializer.RecursionLimit = RecursionLimit.Value;

                }
                response.Write(serializer.Serialize(Data));

            }
        }
    }

    public class DateTimeConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var result = new Dictionary<string, object>();
            FastType ft = FastType.Get(obj.GetType());
            string propertyName = string.Empty;
            foreach (var p in ft.Getters)
            {
                if (p.Info.GetCustomAttribute<ScriptIgnoreAttribute>(true) != null)
                {
                    continue;
                }
                propertyName = p.Info.Name;
                var attr1 = p.Info.GetCustomAttribute<ScriptNameAttribute>(true);
                if(attr1!=null)
                {
                    propertyName = attr1.Name;
                }
                if (p.Type == typeof(DateTime))
                {
                    var attr = p.Info.GetCustomAttribute<TableFieldAttribute>(true);
                    if (attr != null)
                    {
                        string value = string.Format(CultureInfo.CurrentCulture, attr.DataFormatString, new object[] { p.GetValue(obj) });
                        result.Add(propertyName, value);
                    }
                    else
                    {
                        result.Add(propertyName, p.GetValue(obj).ConvertTo<DateTime>().ToString("yyyy年MM月dd日"));
                    }
                }
                else if(p.Type.IsEnum)
                {
                    result.Add(propertyName, p.GetValue(obj).ConvertTo<int>());
                }
                else
                {
                    result.Add(propertyName, p.GetValue(obj));
                }
            }
            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return TypeHelper.Types; }
        }
    }

    public class ComponentListModel
    {
        public string ComponentTypeFullName { get; set; }

        public string ComponentTypeName { get; set; }

        public string ComponentImage { get; set; }

        public string ComponentName { get; set; }

        public string CssClass { get; set; }

        public string Type { get; set; }
    }

    public class ListModel
    {
        public Table Table { get; set; }

        public Search Search { get; set; }

        public Toolbar Toolbar { get; set; }
    }

    public class TreeListModel : ListModel
    {
        public TreeListModel()
        {
            IdParameterName = "TreeId";
        }
        public string TreeDataSource { get; set; }

        public string IdParameterName { get; set; }
    }

    public class FormModel
    {
        public string EntityName { get; set; }

        public Form Form { get; set; }

        public Tab Tab { get; set; }
    }
}
