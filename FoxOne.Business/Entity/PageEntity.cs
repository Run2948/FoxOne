using FoxOne.Data;
using FoxOne.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;
using FoxOne.Core;
using System.Threading;
using System.Web.Script.Serialization;
namespace FoxOne.Business
{
    [Category("应用设计")]
    [DisplayName("页面信息")]
    [Serializable]
    [Table("APP_Page")]
    public class PageEntity : EntityBase, IAutoCreateTable,ILastUpdateTime
    {
        [PrimaryKey]
        [DisplayName("页面地址")]
        public override string Id { get; set; }


        [DisplayName("页面标题")]
        [Column(Length="200")]
        public string Title { get; set; }


        [DisplayName("页面CssClass")]
        public string CssClass { get; set; }

        [DisplayName("页面服务类")]
        public string Service { get; set; }


        [DisplayName("页面Style")]
        [Column(Length = "1000")]
        public string Style { get; set; }


        [DisplayName("页面起始JS")]
        [Column(Length = "2000")]
        public string StartUpScript { get; set; }

        [DisplayName("页面JS方法定义")]
        [Column(DataType="text",Showable=false)]
        public string ScriptBlock { get; set; }

        [DisplayName("页面布局类型")]
        public string LayoutId { get; set; }


        [DisplayName("页面所属父级")]
        public string ParentId { get; set; }


        [DisplayName("页面类型")]
        [Column(Length="20")]
        public string Type { get; set; }

        [DisplayName("最后更新时间")]
        public DateTime LastUpdateTime { get; set; }

        [ScriptIgnore]
        [XmlIgnore]
        public List<ComponentEntity> Components
        {
            get;
            set;
        }

        [ScriptIgnore]
        [XmlIgnore]
        public LayoutEntity Layout
        {
            get;
            set;
        }

        [ScriptIgnore]
        [XmlIgnore]
        public List<ExternalFileEntity> ExtFiles
        {
            get;
            set;
        }
    }

    [DisplayName("页面外置文件")]
    [Table("APP_PageLayoutFile")]
    public class PageLayoutFileEntity : EntityBase, IAutoCreateTable
    {
        [DisplayName("页面")]
        public string PageOrLayoutId { get; set; }

        [DisplayName("文件")]
        public string FileId { get; set; }

        [Column(DataType="varchar",Length="10")]
        public string Type { get; set; }
    }
}
