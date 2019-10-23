using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using FoxOne.Core;
using System.ComponentModel;
namespace FoxOne.Business
{
    [Category("None")]
    [DisplayName("所有缓存键值")]
    public class AllCacheKeyDataSource : ListDataSourceBase
    {
        protected override IEnumerable<IDictionary<string, object>> GetListInner()
        {
            var returnValue = new List<IDictionary<string, object>>();
            string queryString = HttpContext.Current.Request["SearchKey"];
            foreach (var key in CacheHelper.AllKeys)
            {
                if (queryString.IsNullOrEmpty() || key.IndexOf(queryString,StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var dict = new Dictionary<string, object>();
                    dict["Key"] = key;
                    dict["Value"] = CacheHelper.GetValue(key).ToString();
                    returnValue.Add(dict);
                }
            }
            return returnValue;
        }
    }
}
