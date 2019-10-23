using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System
{
    public static class HtmlLangExtension
    {
        public static string Lang(this HtmlHelper helper,string name)
        {
            return ObjectHelper.GetObject<ILangProvider>().GetString(name);
        }
    }
}