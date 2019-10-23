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
    [DisplayName("扩展属性")]
    [Table("SYS_DURPProperty")]
    public class DURPProperty :EntityBase, IDURPProperty, IAutoCreateTable
    {
        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        [Column(Length = "10")]
        public virtual string Status
        {
            get;
            set;
        }


        public string Type
        {
            get;
            set;
        }
    }
}
