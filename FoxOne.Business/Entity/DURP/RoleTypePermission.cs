using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Data.Attributes;
using System.ComponentModel;
namespace FoxOne.Business
{
    [Category("系统管理")]
    [DisplayName("角色类型权限信息")]
    [Table("SYS_RoleTypePermission")]
    public class RoleTypePermission :RelateEntityBase, IRoleTypePermission, IAutoCreateTable
    {
        public string RoleTypeId
        {
            get;
            set;
        }

        public string PermissionId
        {
            get;
            set;
        }
    }
}
