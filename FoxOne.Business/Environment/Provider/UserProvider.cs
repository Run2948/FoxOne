using FoxOne.Business.Security;
using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business.Environment
{
    public class UserProvider:IEnvironmentProvider
    {
        public string Prefix
        {
            get
            {
                return "User";
            }
        }

        public object Resolve(string name)
        {
            object value;
            var fastType = FastType.Get(Sec.User.GetType());
            var getter = fastType.GetGetter(name);
            if (getter != null)
            {
                return getter.GetValue(Sec.User);
            }
            else
            {
                return Sec.User.Properties.TryGetValue(name, out value) ? value : null;
            }
        }
    }
}
