using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.ComponentModel;
namespace FoxOne.Business
{
    public abstract class KeyValueDataSourceBase : ControlBase, IKeyValueDataSource
    {
        private IEnumerable<TreeNode> _Items;
        protected IEnumerable<TreeNode> Items
        {
            get
            {
                return _Items ?? (_Items = SelectItems());
            }
        }

        public abstract IEnumerable<TreeNode> SelectItems();

        public virtual object Converter(string columnName, object columnValue, IDictionary<string, object> rowData)
        {
            if (columnValue != null)
            {
                var item = Items.FirstOrDefault(o => o.Value.Equals(columnValue.ToString(), StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    return item.Text;
                }
            }
            return columnValue;
        }
    }
}
