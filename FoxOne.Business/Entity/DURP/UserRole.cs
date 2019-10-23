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
    [DisplayName("用户角色信息")]
    [Table("SYS_UserRole")]
    public class UserRole : RelateEntityBase, IUserRole, IAutoCreateTable
    {

        public virtual string UserId
        {
            get;
            set;
        }

        public virtual string RoleId
        {
            get;
            set;
        }
    }

    [DisplayName("用户角色信息数据源")]
    public class UserRoleFormService : IFormService
    {

        public int Insert(IDictionary<string, object> data)
        {
            var userId = data["UserId"].ToString();
            var roleId = data["RoleId"].ToString();
            var userIds = userId.Split(',');
            int effectCount = 0;
            foreach (var id in userIds)
            {
                effectCount = DBContext<UserRole>.Insert(new UserRole()
                 {
                     Id = Guid.NewGuid().ToString(),
                     RentId = 1,
                     RoleId = roleId,
                     Status = DefaultStatus.Enabled.ToString(),
                     UserId = id
                 }) ? effectCount + 1 : effectCount;
            }
            return effectCount;
        }

        public int Update(string key, IDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> Get(string key)
        {
            throw new NotImplementedException();
        }

        public int Delete(string key)
        {
            throw new NotImplementedException();
        }

        public string Id
        {
            get;
            set;
        }

        public string PageId
        {
            get;
            set;
        }

        public string ParentId
        {
            get;
            set;
        }

        public string TargetId
        {
            get;
            set;
        }
    }
}
