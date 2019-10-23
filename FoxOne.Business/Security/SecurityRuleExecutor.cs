using System.Collections.Generic;
using FoxOne.Data;

namespace FoxOne.Business.Security
{
    public class SecurityRuleExecutor : ISqlActionExecutor
    {
        public string Execute(ISqlAction action, 
                              ISqlParameters inParams, 
                              IDictionary<string, object> outParams)
        {
            string text = action.Text;

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            else
            {
                string operation;
                string defaultRule;

                int index = text.IndexOf('?');
                if (index > 0)
                {
                    operation = text.Substring(0, index);
                    defaultRule = text.Substring(index + 1);
                }
                else
                {
                    operation   = text;
                    defaultRule = string.Empty;
                }

                string rule = GetPermissionRule(operation);

                return string.IsNullOrEmpty(rule) ? defaultRule : rule;
            }
        }

        protected virtual string GetPermissionRule(string operation)
        {
            return Sec.Provider.GetPermissionRule(operation);
        }

        public string Prefix
        {
            get { return "security"; }
        }
    }
}
