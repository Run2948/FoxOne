using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace FoxOne.Core
{
    public static class CacheHelper
    {
        private static ICache cache;
        private static readonly object lockKey = new object();
        static CacheHelper()
        {
            cache = ObjectHelper.GetObject<ICache>();
        }

        public static IList<string> AllKeys
        {
            get
            {
                return cache.AllKeys;
            }
        }

        public static void SetValue(string key, object value)
        {
            SetValue(key, value, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration);
        }

        public static void SetValue(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            lock (lockKey)
            {
                Remove(key);
                cache.SetValue(key, value, absoluteExpiration, slidingExpiration);
            }
        }

        public static object GetValue(string key)
        {
            return cache.GetValue(key);
        }


        public static bool Remove(string key)
        {
            if (cache.GetValue(key) != null)
            {
                cache.Remove(key);
                return true;
            }
            return false;
        }

        public static void Clean()
        {
            lock (lockKey)
            {
                foreach (var key in cache.AllKeys)
                {
                    cache.Remove(key);
                }
            }
        }

        public static T GetFromCache<T>(string key, Func<T> fun) where T : class
        {
            return GetFromCache<T>(key, fun, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration);
        }

        public static T GetFromCache<T>(string key, Func<T> fun, DateTime absoluteExpiration, TimeSpan slidingExpiration) where T : class
        {
            T returnValue = GetValue(key) as T;
            if (returnValue == null)
            {
                lock (lockKey)
                {
                    returnValue = GetValue(key) as T;
                    if (returnValue == null)
                    {
                        returnValue = fun();
                        if (returnValue != null && !string.IsNullOrEmpty(returnValue.ToString()))
                        {
                            cache.SetValue(key, returnValue, absoluteExpiration, slidingExpiration);
                        }
                    }
                }
            }
            else
            {
                Logger.Debug("cache hit with key:{0}", key);
            }
            return returnValue;
        }
    }
}
