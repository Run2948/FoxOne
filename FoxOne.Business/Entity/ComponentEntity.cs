using FoxOne.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using FoxOne.Core;
using FoxOne.Data;
namespace FoxOne.Business
{
    [Serializable]
    [Category("应用设计")]
    [DisplayName("组件信息")]
    [Table("APP_Component")]
    public class ComponentEntity :EntityBase, IAutoCreateTable,ISortable,ILastUpdateTime
    {
        [PrimaryKey]
        public override string Id { get; set; }

        [PrimaryKey]
        [DisplayName("所属页面")]
        public string PageId { get; set; }

        [DisplayName("所属父级")]
        [Column(Showable = false)]
        public string ParentId { get; set; }

        [DisplayName("组件类型")]
        [Column(DataType = "varchar", Length = "300")]
        public string Type { get; set; }

        [DisplayName("组件XML")]
        [Column(DataType="text",Showable=false)]
        public string JsonContent { get; set; }

        [Column(Showable = false)]
        [DisplayName("组件数据类型")]
        public string DataType { get; set; }

        [DisplayName("排序号")]
        public int Rank { get; set; }

        public string TargetId { get; set; }

        [DisplayName("最后更新时间")]
        [Column(Showable = false)]
        public DateTime LastUpdateTime { get; set; }

        public override bool Equals(object obj)
        {
            var temp = obj as ComponentEntity;
            return temp.Id.Equals(this.Id, StringComparison.OrdinalIgnoreCase) && temp.PageId.Equals(this.PageId, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + PageId.GetHashCode();
        }
    }
}
