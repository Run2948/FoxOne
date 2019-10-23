using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    [DisplayName("枚举转换器")]
    public class EnumColumnConverter:ColumnConverterBase
    {
        /// <summary>
        /// 枚举类型全称
        /// </summary>
        [DisplayName("枚举类型全称")]
        [FunctionDataSource(typeof(AllEnumDataSource))]
        [FormField(ControlType = ControlType.DropDownList)]
        public string EnumTypeFullName { get; set; }

        private EnumDataSource _dataSource;
        protected override IFieldConverter FieldConverter
        {
            get
            {
                return _dataSource ?? (_dataSource = new EnumDataSource() { EnumTypeFullName=EnumTypeFullName});
            }
        }
    }






    [DisplayName("my转换器")]
    public class MyConverter:ControlBase,IFieldConverter
    {
        public object Converter(string columnName, object columnValue, IDictionary<string, object> rowData)
        {
            if(columnValue!=null)
            {
                if(columnValue.ToString().Equals("财务部"))
                {
                    var td = new CustomTd() { Value = columnValue };
                    td.Attribute.Add("style", "background-color:red;");
                    return td;
                } 
            }
            return columnValue;
        }

    }
}
