using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using FoxOne.Core;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Linq;
namespace FoxOne.Business.Security
{
    public class SecurityProvider : ISecurityProvider
    {
        private static readonly string UserSessionKey = typeof(SecurityProvider).FullName + "$User";
        private static readonly string UserPermissionKeyFormat = "{0}_ALL_PERMISSION";

        public virtual bool Authenticate(string username, string password)
        {
            IUser user = DBContext<IUser>.Instance.FirstOrDefault(o => o.LoginId.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (null == user)
            {
                return false;
            }
            else
            {
                return EncryptPassword(password).Equals(user.Password);
            }
        }

        public virtual string EncryptPassword(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
        }

        public bool HasPermissionOfUrl(string virtualPath, string queryString, IUser user = null)
        {
            string virtualUrl = virtualPath + queryString;

            IEnumerable<IPermission> urls = DBContext<IPermission>.Instance.Where(o => o.Url.IsNotNullOrEmpty() && o.Url.Equals(virtualPath, StringComparison.OrdinalIgnoreCase));
            if (urls.Count() > 0)
            {
                return GetAllUserPermission(user).Any(p => p.Url.Equals(virtualPath, StringComparison.OrdinalIgnoreCase));
            }
            return true;
        }

        public bool HasPermission(string operation, IUser user = null)
        {
            return GetAllUserPermission(user).Any(p => p.Code.Equals(operation, StringComparison.OrdinalIgnoreCase));
        }

        public string GetPermissionRule(string operation, IUser user = null)
        {
            IEnumerable<IPermission> rules =GetAllUserPermission(user).Where(o=>o.Parent.Code.Equals(operation, StringComparison.OrdinalIgnoreCase));
            int count = rules.Count();
            if (count  > 0)
            {
                return (count == 1 ? rules.First() : rules.OrderBy(rule => rule.Rank).First()).Url;
            }
            return null;
        }

        public virtual IUser GetCurrentUser()
        {
            IPrincipal principal = HttpContext.Current.User;
            if (null != principal)
            {
                if (principal.Identity.IsAuthenticated)
                {
                    IUser user = GetUser(principal);

                    if (null == user)
                    {
                        throw new InvalidOperationException(
                                    string.Format("Invalid User State,Return Null For Authenticated User '{0}'",
                                                    principal.Identity.Name));
                    }
                    return user;
                }
            }
            return null;
        }

        public virtual void Abandon()
        {
            try
            {
                RemoveUserFromSession();
            }
            catch { }
        }

        public virtual IDictionary<string, UISecurityBehaviour> GetUISecurityBehaviours(string virtualPath, string queryString)
        {
            //获取当前页面中所有受权限控制的控件。
            var permissions = DBContext<IPermission>.Instance.Where(o =>
                o.Type == PermissionType.Control
                && o.Parent.Url.StartsWith(virtualPath, StringComparison.OrdinalIgnoreCase));

            //获取当前用户的所有权限
            var allUserPermission = GetAllUserPermission();

            IDictionary<string, UISecurityBehaviour> behaviours = new Dictionary<string, UISecurityBehaviour>();
            permissions.ForEach(p =>
            {
                IEnumerable<IPermission> tempPermission =
                        allUserPermission.Where(o => o.Code.Equals(p.Code, StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.Rank);
                if (tempPermission.IsNullOrEmpty())
                {
                    behaviours[p.Url] = new UISecurityBehaviour() { Behaviour = UISecurityBehaviour.Invisible };
                }
                else
                {
                    behaviours[p.Url] = new UISecurityBehaviour() { Behaviour = tempPermission.First().Behaviour };
                }
            });
            return behaviours;
        }

        private SessionProvider _session;
        public SessionProvider AppSession
        {
            get
            {
                return _session ?? (_session = new SessionProvider());
            }
        }

        protected virtual IUser GetUser(IPrincipal principal)
        {
            string loginId = GetLoginIdFromPrincipal(principal);
            IUser user = GetUserFromSession(loginId);
            if (null == user)
            {
                user = DBContext<IUser>.Instance.FirstOrDefault(o => o.LoginId.Equals(loginId, StringComparison.OrdinalIgnoreCase));
                if (null == user)
                {
                    throw new Exception(
                        string.Format("User Not Found!", loginId));
                }
                if (AppSession.IsValid)
                {
                    AppSession[UserSessionKey] = user;
                }
            }
            return user;
        }


        protected virtual string GetLoginIdFromPrincipal(IPrincipal principal)
        {
            string name = principal.Identity.Name;
            if (principal.Identity is WindowsIdentity)
            {
                int index = name.IndexOf("\\");
                return index > 0 ? name.Substring(index + 1) : name;
            }
            else
            {
                return name;
            }
        }

        protected virtual IUser GetUserFromSession(string name)
        {
            IUser user = AppSession.IsValid ? (AppSession[UserSessionKey] as IUser) : null;
            if (null != user && !user.LoginId.Equals(name))
            {
                RemoveUserFromSession();
                user = null;
            }
            return user;
        }

        protected virtual void RemoveUserFromSession()
        {
            if (AppSession.IsValid)
            {
                AppSession.Remove(UserPermissionKeyFormat.FormatTo(GetCurrentUser().LoginId));
                AppSession.Remove(UserSessionKey);
            }
        }

        public IEnumerable<IPermission> GetAllUserPermission(IUser user = null)
        {
            user = user ?? (user = GetCurrentUser());
            string key = UserPermissionKeyFormat.FormatTo(user.LoginId);
            IList<IPermission> result = AppSession[key] as IList<IPermission>;
            if (result == null)
            {
                result = new List<IPermission>();
                foreach (var role in user.Roles)
                {
                    role.Permissions.ForEach(p =>
                    {
                        if (result.Count(o => o.Id.Equals(p.Id, StringComparison.OrdinalIgnoreCase)) == 0)
                        {
                            result.Add(p);
                        }
                    });
                }
                AppSession[key] = result;
            }
            return result;
        }


        public bool ResetPassword(string userName, string newPassword)
        {
            IUser user = DBContext<IUser>.Instance.FirstOrDefault(o => o.LoginId.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                throw new FoxOneException("User_Name_Not_Exist");
            }
            user.Password = EncryptPassword(newPassword);
            return DBContext<IUser>.Update(user);
        }
    }
}