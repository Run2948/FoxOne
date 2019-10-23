using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    [DisplayName("数据字典转换器")]
    public class DictionaryColumnConverter : ColumnConverterBase
    {
        [DisplayName("字典编号")]
        [FunctionDataSource(typeof(DictionaryCodeDataSource))]
        [FormField(ControlType = ControlType.DropDownList)]
        public string DictionaryCode { get; set; }

        private DictionaryDataSource _dataSource;

        protected override IFieldConverter FieldConverter
        {
            get
            {
                return _dataSource ?? (_dataSource = new DictionaryDataSource() { DictionaryCode = DictionaryCode });
            }
        }
    }
}
