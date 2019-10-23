using FoxOne.Core;

namespace FoxOne.Business.Security
{
    public sealed class Sec
    {
        private static ISecurityProvider _provider;
        public static IUser User
        {
            get { return Provider.GetCurrentUser(); }
        }

        public static ISecurityProvider Provider
        {
            get 
            {
                return _provider ?? (_provider = new SecurityProvider());
            }
        }
    }
}