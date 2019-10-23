using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FoxOne.Core;
namespace FoxOne.Controls
{
    public abstract class KeyValueControlBase : FormControlBase, IKeyValueDataSourceControl
    {
        public KeyValueControlBase()
            : base()
        {
            AppendEmptyOption = false;
            EmptyOptionText = "全部";
            ChangeTiggerSearch = true;
        }

        /// <summary>
        /// 附加空选项
        /// </summary>
        [DisplayName("附加空选项")]
        public bool AppendEmptyOption { get; set; }

        /// <summary>
        /// 空选项文本
        /// </summary>
        [DisplayName("空选项文本")]
        public string EmptyOptionText { get; set; }


        [DisplayName("数据源")]
        public IKeyValueDataSource DataSource
        {
            get;
            set;
        }

        public string RelateControlID
        {
            get;
            set;
        }

        internal override void AddAttributes()
        {
            if (!RelateControlID.IsNullOrEmpty())
            {
                Attributes["data-relateId"] = RelateControlID;
            }
            base.AddAttributes();
        }

        protected virtual IEnumerable<TreeNode> GetData()
        {
            if (DataSource == null)
            {
                throw new FoxOneException("Need_DataSource", Id);
            }
            var items = DataSource.SelectItems().ToList();
            if (items != null && AppendEmptyOption)
            {
                items.Insert(0, new TreeNode() { Text = EmptyOptionText, Value = "", Checked = Value.IsNullOrEmpty() });
            }
            if (!Value.IsNullOrEmpty())
            {
                items.ForEach((o) =>
                {
                    if (Value.Split(',').Contains(o.Value, StringComparer.OrdinalIgnoreCase))
                    {
                        o.Checked = true;
                    }
                });
            }
            return items;
        }
    }
}
