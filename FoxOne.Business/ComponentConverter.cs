using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using FoxOne.Core;
using System.Web;
namespace FoxOne.Business
{
    public class ComponentConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var instance = Activator.CreateInstance(type);
            FastType ft = FastType.Get(type);
            foreach (var p in ft.Setters)
            {
                if (!dictionary.Keys.Contains(p.Name)) continue;
                if (p.Info.GetCustomAttribute<ScriptIgnoreAttribute>(true) != null
                    || p.Type.IsGenericType
                    || typeof(IControl).IsAssignableFrom(p.Type))
                {
                    continue;
                }
                if (p.Info.GetCustomAttribute<HtmlEncodeAttribute>(true) != null)
                {
                    p.SetValue(instance, HttpUtility.HtmlDecode(dictionary[p.Name].ToString()));
                }
                else
                {
                    p.SetValue(instance, dictionary[p.Name]);
                }
            }
            return instance;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var result = new Dictionary<string, object>();
            FastType ft = FastType.Get(obj.GetType());
            foreach (var p in ft.Getters)
            {
                if (p.Info.GetCustomAttribute<ScriptIgnoreAttribute>(true) != null
                    || p.Type.IsGenericType
                    || typeof(IControl).IsAssignableFrom(p.Type))
                {
                    continue;
                }
                if (p.Info.GetCustomAttribute<HtmlEncodeAttribute>(true) != null)
                {
                    result.Add(p.Name, HttpUtility.HtmlEncode(p.GetValue(obj)));
                }
                else
                {
                    result.Add(p.Name, p.GetValue(obj));
                }
            }
            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return TypeHelper.GetAllImpl<IControl>();
            }
        }
    }
}
