using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Compilation;

namespace FoxOne.Core
{
    public static class Assemblies
    {
        private static readonly ReadOnlyCollection<Assembly> _all = null;

        static Assemblies()
        {
            List<Assembly> all = new List<Assembly>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                //这里只加载了当前运行所需要的程序集，未必是全部。
                AddAssembly(all, a);
            }
            if (HttpContext.Current != null)
            {
                foreach (Assembly a in BuildManager.GetReferencedAssemblies())
                {
                    if (!all.Any(loaded =>
                        AssemblyName.ReferenceMatchesDefinition(loaded.GetName(), a.GetName())))
                    {
                        AddAssembly(all, a);
                    }
                }
                string binDir = HttpRuntime.BinDirectory;
                if (!string.IsNullOrEmpty(binDir))
                {
                    string[] files = Directory.GetFiles(binDir, "*.dll", SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        if (file.StartsWith("FoxOne", StringComparison.OrdinalIgnoreCase))
                        {
                            AssemblyName name = AssemblyName.GetAssemblyName(file);
                            Assembly a = Assembly.Load(name);
                            if (!all.Any(loaded =>
                                AssemblyName.ReferenceMatchesDefinition(loaded.GetName(), name)))
                            {
                                AddAssembly(all, a);
                            }
                        }
                    }
                }
            }
            else
            {
                //为了单元测试
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.IsNullOrEmpty(dir))
                {
                    var dirInfo = new DirectoryInfo(dir);
                    var files = dirInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        if (file.Name.StartsWith("FoxOne", StringComparison.OrdinalIgnoreCase))
                        {
                            AssemblyName name = AssemblyName.GetAssemblyName(file.Name);
                            Assembly a = Assembly.Load(name);
                            if (!all.Any(loaded =>
                                AssemblyName.ReferenceMatchesDefinition(loaded.GetName(), name)))
                            {
                                AddAssembly(all, a);
                            }
                        }
                    }
                }
            }
            _all = new ReadOnlyCollection<Assembly>(all);
        }

        private static void AddAssembly(List<Assembly> all, Assembly a)
        {
            if (a.FullName.StartsWith("FoxOne", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Debug("Load assembly : {0}", a.GetName().Name);
                all.Add(a);
            }
        }

        public static IEnumerable<Assembly> GetAssemblies()
        {
            return _all;
        }
    }
}
