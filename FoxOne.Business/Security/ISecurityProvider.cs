using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using FoxOne.Core;

namespace FoxOne.Business.Security
{
    public interface ISecurityProvider
    {
        bool Authenticate(string username, string password);

        string EncryptPassword(string password);

        bool ResetPassword(string userName, string newPassword);

        bool HasPermissionOfUrl(string virtualPath, string queryString, IUser user = null);

        bool HasPermission(string operation,IUser user=null);

        string GetPermissionRule(string operation,IUser user=null);


        IEnumerable<IPermission> GetAllUserPermission(IUser user = null);

        void Abandon();

        IUser GetCurrentUser();

        IDictionary<string, UISecurityBehaviour> GetUISecurityBehaviours(string virtualPath, string queryString);
    }
}