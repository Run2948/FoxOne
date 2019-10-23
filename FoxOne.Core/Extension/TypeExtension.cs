using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FoxOne.Core
{
    public static class TypeExtension
    {
        public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var obj = type.GetCustomAttributes(typeof(T), inherit);
            if (obj != null)
            {
                return (T)obj.FirstOrDefault();
            }
            return null;
        }

        public static string GetDisplayName(this Type type)
        {
            string result = type.Name;
            var attr = type.GetCustomAttribute<DisplayNameAttribute>(true);
            if(attr!=null)
            {
                result = attr.DisplayName;
            }
            return result;
        }

        public static string GetCategory(this Type type)
        {
            string result = string.Empty;
            var attr = type.GetCustomAttribute<CategoryAttribute>(true);
            if(attr!=null)
            {
                result = attr.Category;
            }
            return result;
        }

        public static string GetDescription(this Type type)
        {
            string result = string.Empty;
            var attr = type.GetCustomAttribute<DescriptionAttribute>(true);
            if (attr != null)
            {
                result = attr.Description;
            }
            return result;
        }
    }
}
