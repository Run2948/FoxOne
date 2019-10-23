using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxOne.Core
{
    public static class ObjectExtension
    {
        public static T ConvertTo<T>(this object value)
        {
            return (T)value.ConvertToType(typeof(T));
        }

        public static object ConvertToType(this object value, Type type)
        {
            if (null == value || value is DBNull ||
                (typeof(string) != type && (value is string) && string.IsNullOrEmpty((string)value)))
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
            Type valueType = value.GetType();
            if (valueType == type || type.IsAssignableFrom(valueType))
            {
                return value;
            }
            if (type.IsPrimitive)
            {
                try
                {
                    return System.Convert.ChangeType(value, type);
                }
                catch { }
            }
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (null != converter)
            {
                if (converter.CanConvertFrom(valueType))
                {
                    return converter.ConvertFrom(value);
                }
                else if (type.IsValueType)
                {
                    return converter.ConvertFrom(value.ToString());
                }
            }
            converter = TypeDescriptor.GetConverter(valueType);
            if (null != converter && converter.CanConvertTo(type))
            {
                return converter.ConvertTo(value, type);
            }
            if (typeof(string).Equals(type))
            {
                return value.ToString();
            }
            throw new InvalidCastException(
                string.Format("Can Not Convert Type '{0}' To '{1}'",
                                value.GetType().FullName, type.FullName));
        }

        public static IDictionary<string, object> ToDictionary(this object value)
        {
            if (value == null) return null;
            IDictionary<string, object> dictionary = null;
            if (typeof(IDictionary<string,object>).IsAssignableFrom( value.GetType()))
            {
                dictionary = value as IDictionary<string, object>;
            }
            else if (value.GetType().IsSubclassOf(typeof(NameValueCollection)))
            {
                var data = value as NameValueCollection;
                dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var key in data.AllKeys)
                {
                    dictionary.Add(key, data[key]);
                }
            }
            else
            {
                dictionary = new FoxOneDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                if(value is IExtProperty)
                {
                    var prop = (value as IExtProperty).Properties;
                    if(!prop.IsNullOrEmpty())
                    {
                        dictionary.AddRange(prop);
                    }
                }
                FastType fastType = FastType.Get(value.GetType());
                fastType.Getters.ForEach(getter =>
                {
                    if (getter.Type.IsArray || getter.Type.IsValueType || getter.Type == typeof(string))
                    {
                        string name = getter.Name;
                        if (!name.IsNullOrEmpty())
                        {
                            object val = getter.GetValue(value);
                            dictionary.Add(name, val);
                        }
                    }
                });
            }
            return dictionary;
        }
    }
}
