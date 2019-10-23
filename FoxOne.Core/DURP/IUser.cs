/*********************************************************
 * 作　　者：刘海峰
 * 联系邮箱：mailTo:liuhf@FoxOne.net
 * 创建时间：2014/12/29 12:52:16
 * 描述说明：
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace FoxOne.Core
{
    public interface IUser : IDURP
    {
        string LoginId { get; set; }

        string Password { get; set; }

        string DepartmentId { get; set; }

        IDepartment Department { get; }

        IEnumerable<IRole> Roles { get;  }

        string MobilePhone { get; set; }

        string QQ { get; set; }

        DateTime Birthdate { get; set; }

        string Sex { get; set; }
    }
}
