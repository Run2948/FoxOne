using FoxOne.Business.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public interface IAuthorityComponent
    {
        void Authority(IDictionary<string, UISecurityBehaviour> behaviour);
    }
}
