using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public interface IColumnConverter : ISortableControl
    {
        string ColumnName { get; set; }

        string AppendColumnName { get; set; }

        ColumnConverterType ConverterType { get; set; }

        IDictionary<string, object> RowData { get; set; }

        object Converter(object value);
    }

    public enum ColumnConverterType
    {
        [Description("叠加")]
        Append,

        [Description("替换")]
        Replace,

        [Description("重命名")]
        Rename,

        [Description("移除")]
        Remove
    }
}
