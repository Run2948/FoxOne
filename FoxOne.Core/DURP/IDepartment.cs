using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IDepartment : IDURP
    {
        string ParentId { get; set; }

        int Level { get; set; }

        string WBS { get; set; }

        IDepartment Parent { get; }

        IEnumerable<IDepartment> Childrens { get; }

        IEnumerable<IUser> Member { get; }

        IEnumerable<IRole> Roles { get; }

    }
}
