using System;
using System.Collections.Generic;
using System.Security.Principal;
using FoxOne.Core;
using System.Linq;
using FoxOne.Data.Attributes;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
namespace FoxOne.Business
{
    [Category("系统管理")]
    [DisplayName("用户信息")]
    [Table("SYS_User")]
    [Serializable]
    public class User : DURPBase, IUser, IAutoCreateTable
    {
        [DisplayName("账号")]
        public virtual string LoginId
        {
            get;
            set;
        }

        [Column(Showable=false)]
        public virtual string Password { get; set; }

        [DisplayName("所属部门")]
        public virtual string DepartmentId { get; set; }

        [ScriptIgnore]
        [XmlIgnore]
        public virtual IEnumerable<IRole> Roles
        {
            get 
            {
                var userRole = DBContext<IUserRole>.Instance.Where(o => o.UserId.IsNotNullOrEmpty() && o.UserId.Equals(Id, StringComparison.OrdinalIgnoreCase)).ToList().Select(o => o.RoleId);
                return DBContext<IRole>.Instance.Where(o => userRole.Contains(o.Id, StringComparer.OrdinalIgnoreCase));
            }
        }

        [ScriptIgnore]
        [XmlIgnore]
        public IDepartment Department
        {
            get 
            {
                return DBContext<IDepartment>.Instance.Get(DepartmentId);
            }
        }


        [DisplayName("手机")]
        [Column(Length="20")]
        public string MobilePhone
        {
            get;
            set;
        }


        [Column(Length="20")]
        public string QQ
        {
            get;
            set;
        }

        [DisplayName("出生日期")]
        public DateTime Birthdate
        {
            get;
            set;
        }

        [DisplayName("性别")]
        [Column(Length="10")]
        public string Sex
        {
            get;
            set;
        }
    }
}