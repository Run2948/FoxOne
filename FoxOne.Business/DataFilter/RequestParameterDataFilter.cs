using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Web;
using System.Collections.Specialized;
using System.ComponentModel;
namespace FoxOne.Business
{
    [DisplayName("URL参数自动过滤器")]
    public class RequestParameterDataFilter : DataFilterBase
    {
        private static IDictionary<Type, ColumnOperator> TypeOperatorMapping = new Dictionary<Type, ColumnOperator>();
        static RequestParameterDataFilter()
        {
            TypeOperatorMapping.Add(typeof(string), new EqualsOperation());
            TypeOperatorMapping.Add(typeof(int), new EqualsOperation());
            TypeOperatorMapping.Add(typeof(long), new EqualsOperation());
            TypeOperatorMapping.Add(typeof(decimal), new EqualsOperation());
            TypeOperatorMapping.Add(typeof(bool), new EqualsOperation());
            TypeOperatorMapping.Add(typeof(DateTime), new DateTimeEqualsOperation());
        }

        protected virtual NameValueCollection Parameter
        {
            get
            {
                switch (ParameterRange)
                {
                    case ParameterRange.QueryString:
                        return HttpContext.Current.Request.QueryString;
                    case ParameterRange.Form:
                        return HttpContext.Current.Request.Form;
                    case ParameterRange.QueryStringAndForm:
                        var result = HttpContext.Current.Request.QueryString;
                        result.Add(HttpContext.Current.Request.Form);
                        return result;
                    default:
                        return HttpContext.Current.Request.QueryString;
                }
            }
        }

        public ParameterRange ParameterRange { get; set; }



        public override bool Filter(IDictionary<string, object> data)
        {
            bool result = true;
            string tempKey = string.Empty;
            foreach (var key in Parameter.AllKeys)
            {
                object value = Parameter[key];
                if (value == null || value.ToString().IsNullOrEmpty())
                {
                    continue;
                }
                if (data.Keys.Contains(key, StringComparer.OrdinalIgnoreCase))
                {
                    result = FilterInner(data[key], value);
                    if (result == false)
                    {
                        break;
                    }
                }
                else
                {
                    tempKey = key.Replace("_", "");
                    if (data.Keys.Contains(tempKey, StringComparer.OrdinalIgnoreCase))
                    {
                        result = FilterInner(data[tempKey], value);
                        if (result == false)
                        {
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private bool FilterInner(object source, object value)
        {
            bool result = false;
            if (source != null)
            {
                ColumnOperator op = null;
                if (TypeOperatorMapping.Keys.Contains(source.GetType()))
                {
                    op = TypeOperatorMapping[source.GetType()];
                }
                else
                {
                    op = TypeOperatorMapping[typeof(string)];
                }
                result = op.Operate(source, value);
            }
            return result;
        }
    }

    public enum ParameterRange
    {
        QueryString,
        Form,
        QueryStringAndForm
    }

}
