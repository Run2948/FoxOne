using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IRoleType:IDURP
    {
        IEnumerable<IPermission> Permissions { get; }

        IEnumerable<IRole> Roles { get; }
    }


}
