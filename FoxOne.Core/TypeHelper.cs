using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FoxOne.Core
{
    public static class TypeHelper
    {
        private static List<Type> _types;
        public static List<Type> Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new List<Type>();
                    var assembilies = Assemblies.GetAssemblies();
                    foreach (var assembly in assembilies)
                    {
                        _types.AddRange(assembly.GetTypes().Where(o => o.IsPublic));
                    }
                }
                return _types;
            }
        }

        /// <summary>
        /// 获取程序集中实现了某接口的所有类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Type> GetAllImpl<T>() where T : class
        {
            return GetAllImpl(typeof(T));
        }

        /// <summary>
        /// 获取程序集中实现了某接口的所有类的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetAllImplInstance<T>() where T : class
        {
            var types = GetAllImpl<T>();
            var result = new List<T>();
            foreach (var type in types)
            {
                try
                {
                    result.Add(Activator.CreateInstance(type) as T);
                }
                catch
                {
                    throw new Exception("create instance error ,type:{0}".FormatTo(type.FullName));
                }
                
            }
            return result;
        }

        /// <summary>
        /// 获取程序集中所有的枚举类
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllEnum()
        {
            var result = new List<Type>();
            foreach (var type in Types)
            {
                if (type.IsEnum)
                {
                    result.Add(type);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定类型集合中实现某接口的所有类（不包含抽象类）
        /// </summary>
        /// <param name="interfaceType">接口</param>
        /// <param name="types">指定类型集合，可为空，为空则为程序集中所有类型</param>
        /// <returns></returns>
        public static List<Type> GetAllImpl(Type interfaceType, List<Type> types = null)
        {
            var result = new List<Type>();
            if (types == null)
            {
                types = Types;
            }
            foreach (var type in types)
            {
                if (interfaceType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    result.Add(type);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定类型集合中某基类的所有继承类（不包含抽象类）
        /// </summary>
        /// <typeparam name="T">基类</typeparam>
        /// <param name="types">指定类型集合，可为空，为空则为程序集中所有类型</param>
        /// <returns></returns>
        public static List<Type> GetAllSubType<T>(List<Type> types = null)
        {
            return GetAllSubType(typeof(T), types);
        }

        /// <summary>
        /// 获取指定类型集合中某基类的所有继承类（不包含抽象类）
        /// </summary>
        /// <param name="baseType">基类</param>
        /// <param name="types">指定类型集合，可为空，为空则为程序集中所有类型</param>
        /// <returns></returns>
        public static List<Type> GetAllSubType(Type baseType, List<Type> types = null)
        {
            var result = new List<Type>();
            if (types == null)
            {
                types = Types;
            }
            foreach (var type in types)
            {
                if ((type.IsSubclassOf(baseType) || type == baseType) && !type.IsAbstract)
                {
                    result.Add(type);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定类型集合中某基类的所有继承类的实例（不包含抽象类）
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static List<T> GetAllSubTypeInstance<T>(List<Type> types = null) where T : class
        {
            var allTypes = GetAllSubType(typeof(T), types);
            var result = new List<T>();
            foreach (var type in allTypes)
            {
                result.Add(Activator.CreateInstance(type) as T);
            }
            return result;
        }

        public static Type GetType(string typeName)
        {
            if (typeName.IndexOf(',') > 0)
            {
                typeName = typeName.Split(',')[0];
            }
            foreach (var type in Types)
            {
                if (type.FullName.Equals(typeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return type;
                }
            }
            return null;
        }
    }
}
