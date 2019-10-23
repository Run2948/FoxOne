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
    [DisplayName("数据字典")]
    [Table("SYS_DataDictionary")]
    public class DataDictionary : DURPBase, IAutoCreateTable
    {
        public string ParentId 
        { 
            get; 
            set; 
        }

        [ScriptIgnore]
        [XmlIgnore]
        public IList<DataDictionary> Items
        {
            get
            {
                return DBContext<DataDictionary>.Instance.Where(o => o.ParentId.IsNotNullOrEmpty() && o.ParentId.Equals(Id, StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.Rank).ToList();
            }
        }
    }
}
