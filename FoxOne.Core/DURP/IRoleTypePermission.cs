using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IRoleTypePermission : IEntity
    {
        string RoleTypeId { get; set; }

        string PermissionId { get; set; }
    }
}
