using System.Linq;
using System.Web;
using FoxOne.Core;
using FoxOne.Data;
namespace FoxOne.Business.Environment
{
    public class HttpContextProvider : IEnvironmentProvider, ISqlParameters
    {
        public string Prefix
        {
            get
            {
                return "QueryString|Form";
            }
        }

        public object Resolve(string name)
        {
            object value;
            return TryResolve(name, out value) ? value : null;
        }

        public bool TryResolve(string name, out object value)
        {
            if (name.IndexOf(":") > 0)
            {
                name = name.Substring(name.IndexOf(":"));
            }
            var result = false;
            value = null;
            var request = HttpContext.Current.Request;
            if (request.Form.AllKeys.Contains(name))
            {
                value = request.Form[name];
                result = true;
            }
            if (!result)
            {
                if (request.QueryString.AllKeys.Contains(name))
                {
                    value = request.QueryString[name];
                    result = true;
                }
            }
            if (result && value != null && value.ToString().IsNotNullOrEmpty())
            {
                value = HttpUtility.UrlDecode(value.ToString());
            }
            return result;
        }
    }
}
