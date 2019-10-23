using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public interface IDataFilter : ISortableControl
    {
        bool Filter(IDictionary<string, object> data);
    }

    public abstract class ColumnOperator:ControlBase
    {
        public abstract bool Operate(object obj1, object obj2);
    }

    public enum OperatorType
    {
        [Description("与")]
        And,

        [Description("或")]
        Or
    }
}
