using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxOne.Core
{
    public static class SysConfig
    {

        static SysConfig()
        {
            AppSettings = new AppSettingPropery();
            SystemTitle = AppSettings["SystemTitle"];
            CopyRightName = AppSettings["CopyRightName"];
            Assemblies = AppSettings["Assemblies"];
            SystemStatus = AppSettings["SystemStatus"];
        }

        public static string ExtFieldName = "_ExtField_";

        public static AppSettingPropery AppSettings { get; private set; }

        public static string SystemTitle { get; private set; }

        public static string CopyRightName { get; private set; }

        public static string SystemVersion { get; private set; }

        /// <summary>
        /// 系统当前状态：Develop,Test,Run
        /// </summary>
        public static string SystemStatus { get; private set; }


        public static string Assemblies { get; private set; }

        public static string IconBasePath { get { return "/images/icons/"; } }

        public static string ControlImageBasePath { get { return "/images/Controls/"; } }
    }

    public class AppSettingPropery : Dictionary<string, string>
    {
        private AppSettingsReader _reader;
        private AppSettingsReader Reader
        {
            get
            {
                return _reader ?? (_reader = new AppSettingsReader());
            }
        }

        public new string this[string key]
        {
            get
            {
                if (!base.Keys.Contains(key))
                {
                    base[key] = Reader.GetValue(key, typeof(string)) as string;
                }
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
