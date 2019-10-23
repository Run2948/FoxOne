using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IUserRole : IEntity
    {
        string UserId { get; set; }

        string RoleId { get; set; }
    }
}
