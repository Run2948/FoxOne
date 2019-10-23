using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace FoxOne.Core
{
    public static class Utility
    {
        public const string CONFIG_DIRECTORY_NAME = "App_Config";

        public static T GetConfigSection<T>(string sectionName, string configFileName) where T : ConfigurationSection
        {
            FileInfo configFile;

            if (FindConfigFile(configFileName, out configFile))
            {
                ExeConfigurationFileMap map =
                    new ExeConfigurationFileMap() { ExeConfigFilename = configFile.FullName };

                Configuration configuration =
                    ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                return configuration.GetSection(sectionName) as T;
            }
            else
            {
                return ConfigurationManager.GetSection(sectionName) as T;
            }
        }

        public static bool FindConfigFile(string fileName, out FileInfo fileInfo)
        {
            DirectoryInfo dirInfo = null;
            string file = fileName;
            if (!Path.IsPathRooted(fileName))
            {
                if (FindConfigDirectory(out dirInfo))
                {
                    file = dirInfo.FullName + "\\" + fileName;
                }
            }

            if (File.Exists(file))
            {
                fileInfo = new FileInfo(file);
                return true;
            }
            else
            {
                fileInfo = null;
                return false;
            }
        }

        public static bool FindConfigDirectory(string dirName, out DirectoryInfo dirInfo)
        {
            if (FindConfigDirectory(out dirInfo))
            {
                string dir = dirInfo.FullName + "\\" + dirName;

                if (!Directory.Exists(dir))
                {
                    dirInfo = null;
                    return false;
                }
                else
                {
                    dirInfo = new DirectoryInfo(dir);
                    return true;
                }
            }
            return false;
        }

        private static bool FindConfigDirectory(out DirectoryInfo dirInfo)
        {
            dirInfo = null;
            string dir = CONFIG_DIRECTORY_NAME;
            if (HttpContext.Current != null)
            {
                dir = HttpContext.Current.Server.MapPath("~") + "\\" + dir;
            }
            else
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                dir = baseDir + "\\" + dir;
            }

            if (!Directory.Exists(dir))
            {
                return false;
            }
            else
            {
                dirInfo = new DirectoryInfo(dir);
            }
            return true;
        }
    }
}
