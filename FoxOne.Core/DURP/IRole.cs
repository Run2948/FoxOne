/*********************************************************
 * 作　　者：刘海峰
 * 联系邮箱：mailTo:liuhf@FoxOne.net
 * 创建时间：2015/1/26 8:35:53
 * 描述说明：
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    /// <summary>
    /// 角色接口定义
    /// </summary>
    public interface IRole : IEntity
    {
        string RoleTypeId { get; set; }

        string DepartmentId { get; set; }

        IRoleType RoleType { get; }

        IEnumerable<IUser> Members { get; }

        IEnumerable<IPermission> Permissions { get; }
    }
}
