using FoxOne.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
namespace FoxOne.Business
{
    [Category("应用设计")]
    [DisplayName("布局信息")]
    [Serializable]
    [Table("APP_Layout")]
    public class LayoutEntity : EntityBase, IAutoCreateTable, ILastUpdateTime
    {
        [PrimaryKey]
        [TitleField]
        [DisplayName("布局名称")]
        public override string Id
        {
            get;
            set;
        }

        public string Title { get; set; }

        [Column(DataType="text")]
        [DisplayName("布局HTML")]
        public string Html { get; set; }

        [Column(Length="2000")]
        [DisplayName("布局启动脚本")]
        public string StartUpScript { get; set; }

        [Column(Length="500")]
        [DisplayName("布局样式")]
        public string Style { get; set; }

        [DisplayName("最后更新时间")]
        [FormField(Editable = false)]
        public DateTime LastUpdateTime { get; set; }

        [ScriptIgnore]
        [XmlIgnore]
        public List<ExternalFileEntity> ExtFiles
        {
            get;
            set;
        }
    }
}
