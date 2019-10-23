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
    [DisplayName("角色权限信息")]
    [Table("SYS_RolePermission")]
    public class RolePermission :RelateEntityBase, IRolePermission, IAutoCreateTable
    {
        public virtual string RoleId
        {
            get;
            set;
        }

        public virtual string PermissionId
        {
            get;
            set;
        }
    }
}
