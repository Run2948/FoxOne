using FoxOne.Core;
using FoxOne.Data;
using FoxOne.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    [Category("应用设计")]
    [DisplayName("增删改查")]
    [Table("APP_CRUD")]
    public class CRUDEntity : EntityBase, IAutoCreateTable, ILastUpdateTime
    {
        [DisplayName("主键名")]
        public string PKName { get; set; }

        [DisplayName("标题字段")]
        public string TitleField { get; set; }

        public string ValueField { get; set; }

        public string ParentField { get; set; }

        [DisplayName("表名")]
        public string TableName { get; set; }

        [Column(DataType="text")]
        public string InsertSQL { get; set; }

        [Column(DataType = "text")]
        public string UpdateSQL { get; set; }

        [Column(DataType = "text")]
        public string DeleteSQL { get; set; }

        [Column(DataType = "text")]
        public string GetOneSQL { get; set; }

        [Column(DataType = "text")]
        public string SelectSQL { get; set; }

        public string DefaultSortExpression { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}
