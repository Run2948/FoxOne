using FoxOne.Core;
using FoxOne.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
namespace FoxOne.Business
{
    [Category("系统管理")]
    [DisplayName("角色信息")]
    [Table("SYS_Role")]
    [Serializable]
    public class Role : RelateEntityBase, IRole, IAutoCreateTable
    {

        public string RoleTypeId
        {
            get;
            set;
        }

        public string DepartmentId
        {
            get;
            set;
        }

        [ScriptIgnore]
        [XmlIgnore]
        public IEnumerable<IUser> Members
        {
            get
            {
                var userRole = DBContext<IUserRole>.Instance.Where(o => o.RoleId!=null && o.RoleId.Equals(Id, StringComparison.OrdinalIgnoreCase)).ToList().Select(o => o.UserId);
                return DBContext<IUser>.Instance.Where(o => userRole.Contains(o.Id, StringComparer.OrdinalIgnoreCase));
            }
        }

        [ScriptIgnore]
        [XmlIgnore]
        public IEnumerable<IPermission> Permissions
        {
            get
            {
                var rolePermission = DBContext<IRolePermission>.Instance.Where(o => o.RoleId != null && o.RoleId.Equals(Id, StringComparison.OrdinalIgnoreCase)).ToList().Select(o => o.PermissionId);
                var permission = DBContext<IPermission>.Instance.Where(o => rolePermission.Contains(o.Id, StringComparer.OrdinalIgnoreCase));
                if(permission.IsNullOrEmpty())
                {
                    permission = new List<IPermission>();
                }
                var pp = permission.ToList();
                if(RoleType.Permissions.Count()>0)
                {
                    RoleType.Permissions.ForEach(p => { 
                        if(pp.Count(o=>o.Id.Equals(p.Id, StringComparison.OrdinalIgnoreCase))==0)
                        {
                            pp.Add(p);
                        }
                    });
                }
                return pp;
            }
        }

        [ScriptIgnore]
        [XmlIgnore]
        public IRoleType RoleType
        {
            get {
                return DBContext<IRoleType>.Instance.Get(RoleTypeId);
            }
        }
    }
}