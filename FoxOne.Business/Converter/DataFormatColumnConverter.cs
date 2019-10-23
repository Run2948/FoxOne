using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.ComponentModel;
using System.Globalization;
namespace FoxOne.Business
{
    [DisplayName("格式化转换器")]
    public class DataFormatColumnConverter:ColumnConverterBase
    {
        [DisplayName("输出格式")]
        public string DataFormatString { get; set; }

        [DisplayName("用于格式化的列")]
        public string DataFormatFields { get; set; }

        public override object Converter(object value)
        {
            if (!DataFormatString.IsNullOrEmpty())
            {
                if (!DataFormatFields.IsNullOrEmpty())
                {
                    var dataFields = DataFormatFields.Split(',');
                    object[] param = new object[dataFields.Length];
                    for (int i = 0; i < dataFields.Length; i++)
                    {
                        param[i] = RowData[dataFields[i]];
                    }
                    value = string.Format(CultureInfo.CurrentCulture, DataFormatString, param);
                }
                else
                {
                    value = string.Format(CultureInfo.CurrentCulture, DataFormatString, new object[] { value });
                }
            }
            return value;
        }
    }
}
