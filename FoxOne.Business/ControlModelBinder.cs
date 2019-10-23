using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using FoxOne.Core;
namespace FoxOne.Business
{
    [ValidateInput(false)]
    public class ControlModelBinder:IModelBinder
    {
        public const string EntityFullNameHiddenName = "ENTITY_TYPE_FULL_NAME";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.HttpContext.Request.Form;
            string entityTypeFullName = request[EntityFullNameHiddenName];
            if(entityTypeFullName.IsNullOrEmpty())
            {
                throw new FoxOneException("Missing_ControlType_FullName");
            }
            var type = TypeHelper.GetType(entityTypeFullName);
            object entity = Activator.CreateInstance(type);
            var pis = FastType.Get(type).Setters;
            foreach(var p in pis)
            {
                if(request.AllKeys.Contains(p.Name,StringComparer.Create(Thread.CurrentThread.CurrentCulture,true)))
                {
                    var requestValue = request[p.Name];
                    p.SetValue(entity, requestValue.ConvertToType(p.Info.PropertyType));
                }
            }
            return entity;
        }
    }
}
