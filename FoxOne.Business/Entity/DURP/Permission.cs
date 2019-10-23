using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Data.Attributes;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
namespace FoxOne.Business
{
    [Category("系统管理")]
    [DisplayName("权限信息")]
    [Table("SYS_Permission")]
    public class Permission : DURPBase, IPermission, IAutoCreateTable
    {
        [Column(Length="500")]
        public string Url
        {
            get;
            set;
        }

        [Column(Showable = false)]
        public string ParentId
        {
            get;
            set;
        }

        [Column(Length="20")]
        public string Behaviour { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        public IPermission Parent
        {
            get
            {
                return DBContext<IPermission>.Instance.Get(ParentId);
            }
        }

        [Column(Length = "200", Showable = false)]
        public string Icon { get; set; }

        [Column(Length = "500",Showable=false)]
        public string Description { get; set; }

        
        public PermissionType Type
        {
            get;
            set;
        }

        [XmlIgnore]
        [ScriptIgnore]
        public IEnumerable<IPermission> Childrens
        {
            get 
            {
                if (ParentId.IsNotNullOrEmpty())
                {
                    return DBContext<IPermission>.Instance.Where(o => o.ParentId.IsNotNullOrEmpty() && o.ParentId.Equals(Id, StringComparison.OrdinalIgnoreCase));
                }
                return null;
            }
        }
    }
}
