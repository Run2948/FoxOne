using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FoxOne.Core;
using System.Text.RegularExpressions;
namespace FoxOne.Business.Environment
{
    public class EnvironmentContainer
    {
        private readonly IList<IEnvironmentProvider> _providers = TypeHelper.GetAllImplInstance<IEnvironmentProvider>();
        public static readonly Regex Pattern = new Regex(@"\$(?<Variable>.+?)\$", RegexOptions.Compiled);
        public string Parse(string expression)
        {
            if (string.IsNullOrEmpty(expression) || Pattern.Matches(expression).Count==0)
            {
                return expression;
            }
            StringBuilder builder = new StringBuilder();
            var arr = Pattern.Split(expression);
            object value;
            for(int i=0;i<arr.Length;i++)
            {
                //builder.Append(arr[i++]);
                if (TryResolve(arr[i], out value))
                {
                    builder.Append(value == null ? string.Empty : value.ToString());
                }
                else
                {
                    builder.Append(arr[i]);
                }
            }
            return builder.ToString();
        }
        public virtual bool TryResolve(string expression, out object value)
        {
            value = expression;
            if (expression.IsNullOrEmpty()) return false;
            if (expression.IndexOf(".") < 0)
            {
                //默认给DefaultProvider
                expression = "Default.{0}".FormatTo(expression);
            }
            string[] arrs = expression.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string prefix = arrs[0];
            string name = arrs[1];
            IEnvironmentProvider provider = _providers.FirstOrDefault(o => o.Prefix.Split('|').Contains(prefix,StringComparer.OrdinalIgnoreCase));
            if (provider != null)
            {
                value = provider.Resolve(name);
                if(value==null)
                {
                    return false;
                }
                return true;
            }
            value = null;
            return false;
        }
    }
}