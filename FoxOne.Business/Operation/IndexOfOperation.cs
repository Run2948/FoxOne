using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.ComponentModel;
namespace FoxOne.Business
{
    [DisplayName("包含")]
    public class IndexOfOperation:ColumnOperator
    {
        public override bool Operate(object obj1, object obj2)
        {
            if (obj1 == null || obj1.ToString().IsNullOrEmpty()) return false;
            if (obj2 == null || obj2.ToString().IsNullOrEmpty()) return false;
            return obj1.ToString().IndexOf(obj2.ToString()) >= 0;
        }
    }

    [DisplayName("包含于")]
    public class BeIndexOfOperation:IndexOfOperation
    {
        public override bool Operate(object obj1, object obj2)
        {
            return base.Operate(obj2, obj1);
        }
    }
}
