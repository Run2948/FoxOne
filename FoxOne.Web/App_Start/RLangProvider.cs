using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoxOne.Web
{
    public class RLangProvider : ILangProvider
    {
        public string GetString(string name)
        {
            string result = FoxOneTips.ResourceManager.GetString(name);
            if (result.IsNullOrEmpty())
            {
                result = name;
            }
            return result;
        }
    }
}