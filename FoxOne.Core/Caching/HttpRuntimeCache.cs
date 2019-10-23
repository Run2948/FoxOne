using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace FoxOne.Core
{
    public class HttpRuntimeCache : ICache
    {
        private readonly System.Web.Caching.Cache _cache;
        public HttpRuntimeCache()
        {
            _cache = HttpContext.Current.Cache;
        }
        public IList<string> AllKeys
        {
            get
            {
                IDictionaryEnumerator enumerator = this._cache.GetEnumerator();
                List<string> list = new List<string>();
                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Key.ToString());
                }
                return list;
            }
        }

        public void Clean()
        {
            foreach (string str in AllKeys)
            {
                this._cache.Remove(str);
            }

        }

        public object GetValue(string key)
        {
            return this._cache[key];
        }

        public void Remove(string key)
        {
            this._cache.Remove(key);
        }


        public void SetValue(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            if (!string.IsNullOrEmpty(key) && (value != null))
            {
                this._cache.Add(key, value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, null);
            }
        }
    }
}
