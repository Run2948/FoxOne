using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public interface IFieldConverter : IControl
    {
        /// <summary>
        /// 字段转换方法
        /// </summary>
        /// <param name="columnName">字段名</param>
        /// <param name="columnValue">字段值</param>
        /// <param name="rowData">行值（仅用于TableColumn转换时有效）</param>
        /// <returns>转换后的值</returns>
        object Converter(string columnName, object columnValue, IDictionary<string, object> rowData);
    }

    public class CustomTd
    {
        private IDictionary<string, string> _attribute;
        public CustomTd()
        {
            _attribute = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Attribute { get { return _attribute; } }

        public object Value { get; set; }
    }
}
