using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Data.Attributes;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
namespace FoxOne.Business
{
    [Category("系统管理")]
    [DisplayName("角色类型信息")]
    [Table("SYS_RoleType")]
    public class RoleType : DURPBase, IRoleType, IAutoCreateTable
    {
        [ScriptIgnore]
        [XmlIgnore]
        public IEnumerable<IPermission> Permissions
        {
            get
            {
                var rolePermission = DBContext<IRoleTypePermission>.Instance.Where(o => o.RoleTypeId.IsNotNullOrEmpty() && o.RoleTypeId.Equals(Id, StringComparison.OrdinalIgnoreCase)).ToList().Select(o => o.PermissionId);
                return DBContext<IPermission>.Instance.Where(o => rolePermission.Contains(o.Id, StringComparer.OrdinalIgnoreCase));
            }
        }

        [ScriptIgnore]
        [XmlIgnore]
        public IEnumerable<IRole> Roles
        {
            get
            {
                return DBContext<IRole>.Instance.Where(o => o.RoleTypeId.IsNotNullOrEmpty() && o.RoleTypeId.Equals(Id, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
