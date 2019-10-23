using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Security;

namespace FoxOne.Business.Environment
{
    public class AppSettingsProvider : IEnvironmentProvider
    {

        public string Prefix
        {
            get
            {
                return "AppSetting";
            }
        }
        public object Resolve(string name)
        {
            return ConfigurationManager.AppSettings[name];
        } 
    }
}