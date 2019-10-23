using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
namespace FoxOne.Core
{
    public static class JSONHelper
    {
        public static T Deserialize<T>(string jsonStr)
        {
            if (jsonStr.IsNullOrEmpty())
            {
                throw new ArgumentNullException("jsonStr");
            }
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(jsonStr);
        }

        public static object Deserialize(string input, Type targetType)
        {
            if (input.IsNullOrEmpty())
            {
                throw new ArgumentNullException("input");
            }
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize(input, targetType);
        }

        public static string Serialize(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(value);
        }
    }
}
