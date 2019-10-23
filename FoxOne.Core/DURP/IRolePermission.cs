using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IRolePermission : IEntity
    {
        string RoleId { get; set; }

        string PermissionId { get; set; }
    }
}
