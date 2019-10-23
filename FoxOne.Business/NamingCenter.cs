using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
namespace FoxOne.Business
{
    public static class NamingCenter
    {
        public static string PARAM_PAGE_ID = "_PAGE_ID";
        public static string PARAM_CTRL_ID = "_CTRL_ID";
        public static string PARAM_PARENT_ID = "_PARENT_ID";
        public static string PARAM_TARGET_ID = "_TARGET_ID";
        public static string PARAM_KEY_NAME = "_FORM_KEY";
        public static string PARAM_WIDGET_TYPE = "_WIDGET_TYPE";
        public static string PARAM_TYPE_NAME = "_TYPE_NAME";
        public static string PARAM_BASE_TYPE = "_BASE_TYPE";
        public static string PARAM_FORM_VIEW_MODE = "_FORM_VIEW_MODE";
        public static string PARAM_PAGE_INDEX = "_PAGE_INDEX";
        public static string PARAM_PAGE_SIZE = "_PAGE_SIZE";
        public static string PARAM_SORT_EXPRESSION = "_SORT_EXPRESSION";
        public static string CTRL_LIST_URL = "/PageDesigner/ControlList";
        public static string CTRL_SELECT_LIST_URL = "/PageDesigner/ComponentList";
        public static string CTRL_EDIT_URL = "";
        public static string PAGE_LIST_URL = "PageEntityList";
        public static string PAGE_EDIT_URL = "";
        public static string GetEntityTableId(Type entityType)
        {
            return "{0}_Table".FormatTo(entityType.FullName.Replace('.', '_'));
        }

        public static string GetEntityFormId(Type entityType)
        {
            return "{0}_Form".FormatTo(entityType.FullName.Replace('.', '_'));
        }

        public static string GetEntitySearchId(Type entityType)
        {
            return "{0}_Search".FormatTo(entityType.FullName.Replace('.', '_'));
        }

        public static string GetEntityToolbarId(Type entityType)
        {
            return "{0}_Toolbar".FormatTo(entityType.FullName.Replace('.', '_'));
        }



        public static string GetTimeRangeDatePickerStartId(string propertyName)
        {
            return "sdt{0}".FormatTo(propertyName);
        }

        public static string GetTimeRangeDatePickerEndId(string propertyName)
        {
            return "edt{0}".FormatTo(propertyName);
        }

        public static string GetCacheKey(CacheType cacheType,string key)
        {
            return "{0}_{1}".FormatTo(cacheType.ToString(), key);
        }
    }
}
