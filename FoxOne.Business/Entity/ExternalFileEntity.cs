using FoxOne.Core;
using FoxOne.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    [Category("应用设计")]
    [DisplayName("外置文件")]
    [Table("APP_ExternalFile")]
    public class ExternalFileEntity : EntityBase, IAutoCreateTable,ISortable
    {

        [TitleField]
        [DisplayName("文件名称")]
        public string Name { get; set; }

        [DisplayName("文件路径")]
        public string Path { get; set; }

        [DisplayName("文件类型")]
        public string Type { get; set; }

        [DisplayName("排序号")]
        public int Rank { get; set; }
    }
}
