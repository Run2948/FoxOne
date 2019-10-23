using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using System.IO;
using System.Reflection;
namespace FoxOne.Core
{
    public static class ObjectHelper
    {
        private static IUnityContainer container;
        private const string UNITY_DIRECTORY_NAME = "Unity";
        static ObjectHelper()
        {
            Init();
        }

        public static void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            container.RegisterType<TFrom, TTo>();
        }

        public static void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            container.RegisterType<TFrom, TTo>(name);
        }

        public static void RegisterType(Type from, Type to, params InjectionMember[] injectionMembers)
        {
            container.RegisterType(from, to, injectionMembers);
        }

        public static T GetObject<T>() where T : class
        {
            return container.Resolve<T>();
        }

        public static T GetObject<T>(string name) where T : class
        {
            return container.Resolve<T>(name);
        }

        public static object GetObject(Type type)
        {
            return container.Resolve(type);
        }

        /// <summary>
        /// only return named registion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllObjects<T>() where T : class
        {
            return container.ResolveAll<T>();
        }

        public static Type GetRegisterType(Type type)
        {
            if (type.IsInterface)
            {
                var instance = container.Resolve(type);
                if (instance != null)
                {
                    return instance.GetType();
                }
            }
            return type;
        }

        public static Type GetRegisterType<TFrom>()
        {
            return GetRegisterType(typeof(TFrom));
        }

        private static void Init()
        {
            container = new UnityContainer();
            DirectoryInfo dir = null;
            if (Utility.FindConfigDirectory(UNITY_DIRECTORY_NAME, out dir))
            {
                IEnumerable<FileInfo> files = dir.GetFiles("*.config", SearchOption.AllDirectories);
                IEnumerable<Assembly> assemblies = Assemblies.GetAssemblies();
                foreach (FileInfo file in files)
                {
                    try
                    {
                        UnityConfigurationSection section = Utility.GetConfigSection<UnityConfigurationSection>("unity", file.FullName);
                        if (null != section)
                        {
                            if (section.Containers.Count > 1)
                            {
                                throw new FoxOneException("Multiple '<container>' Element Not Supported in {0}", file.FullName);
                            }
                            if (section.Containers.Count != 0)
                            {
                                foreach (var assembly in assemblies)
                                {
                                    section.Assemblies.Add(new AssemblyElement() { Name = assembly.GetName().Name });
                                }
                                section.Configure(container);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        string message = string.Format("Load Unity Config '{0}' Error : {1}", file.Name, e.Message);
                        throw new FoxOneException(message, e);
                    }
                }
            }
        }
    }
}
