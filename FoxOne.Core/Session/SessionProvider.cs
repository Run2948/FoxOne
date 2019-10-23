using System;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;

namespace FoxOne.Core
{
    public class SessionProvider
    {
        private static readonly string UserSessionKeyFormat = typeof (SessionProvider).FullName + "${0}";

        public bool IsValid
        {
            get { return null != HttpContext.Current && null != HttpContext.Current.Session; }
        }

        public object this[string name]
        {
            get
            {
                return SessionSate[name];
            }
            set
            {
                SessionSate[name] = value;
            }
        }

        public bool Remove(string name)
        {
            return SessionSate.Remove(name);
        }

        public void Clear()
        {
            CheckSessionValid();
            string key = GetCacheKey();
            SessionState state = HttpContext.Current.Session[key] as SessionState;
            if (null != state)
            {
                HttpContext.Current.Session.Remove(key);
                state.Dispose();
            }
        }

        private string GetCacheKey()
        {
            string name = HttpContext.Current.User.Identity.Name;
            return string.Format(UserSessionKeyFormat, name);
        }

        protected SessionState SessionSate
        {
            get
            {
                CheckSessionValid();
                string key = GetCacheKey();
                SessionState state = HttpContext.Current.Session[key] as SessionState;
                if (null == state)
                {
                    lock (this)
                    {
                        if ((state = HttpContext.Current.Session[key] as SessionState) == null)
                        {
                            state = new SessionState();
                            HttpContext.Current.Session[key] = state;
                        }
                    }
                }
                return state;
            }
        }
        protected virtual void CheckSessionValid()
        {
            if (null == HttpContext.Current.Session)
            {
                throw new InvalidOperationException("Asp.Net Session Object is Null,Make sure 'EnableSessionState' is true in your .aspx page");
            }
        }
    }
}