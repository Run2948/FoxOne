using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Business.Environment;
using System.ComponentModel;
namespace FoxOne.Business
{
    [DisplayName("变量过滤器")]
    public class StaticDataFilter:DataFilterBase
    {
        private static IDictionary<string, ColumnOperator> OperatorMapping = new Dictionary<string, ColumnOperator>(StringComparer.OrdinalIgnoreCase);

        static StaticDataFilter()
        {
            foreach(var item in TypeHelper.GetAllSubTypeInstance<ColumnOperator>())
            {
                OperatorMapping.Add(item.GetType().FullName, item);
            }
        }
        public string ColumnName { get; set; }

        [DisplayName("运算符")]
        [FormField(ControlType= ControlType.DropDownList)]
        [TypeDataSource(typeof(ColumnOperator))]
        public string Operator { get; set; }

        public override bool Filter(IDictionary<string, object> data)
        {
            if(ColumnName.IsNullOrEmpty())
            {
                throw new FoxOneException("ColumnName_Is_Empty");
            }
            bool result = true;
            if (data.IsNullOrEmpty())
            {
                result = false;
            }
            else
            {
                if (AppendType == FilterAppendType.NotNullOrEmpty)
                {
                    if (Value.IsNullOrEmpty()) return true;
                }
                if (data.Keys.Contains(ColumnName, StringComparer.OrdinalIgnoreCase))
                {
                    var columnOperator = OperatorMapping[Operator];
                    var obj1 = data[ColumnName];
                    object obj2;
                    if(Value.IsNotNullOrEmpty() && Env.TryResolve(Value,out obj2))
                    {
                        if (AppendType == FilterAppendType.NotNullOrEmpty)
                        {
                            if (obj2==null || obj2.ToString().IsNullOrEmpty()) return true;
                        }
                        result = columnOperator.Operate(obj1, obj2);
                    }
                    else
                    {
                        result = columnOperator.Operate(obj1, Value);
                    }
                }
            }
            return result;
        }

        [DisplayName("过滤值")]
        [Description("可使用环境变量表达式")]
        public string Value
        {
            get;
            set;
        }

        [DisplayName("附加类型")]
        public FilterAppendType AppendType
        {
            get;
            set;
        }
    }


    public enum FilterAppendType
    {
        [Description("固定")]
        Static,

        [Description("不为空")]
        NotNullOrEmpty
    }

}
