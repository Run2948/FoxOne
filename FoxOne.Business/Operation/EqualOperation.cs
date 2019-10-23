using FoxOne.Core;
/*********************************************************
 * 作　　者：刘海峰
 * 联系邮箱：mailTo:liuhf@foxone.net
 * 创建时间：2014/6/8 17:48:43
 * 描述说明：
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FoxOne.Business
{
    [DisplayName("等于")]
    public class EqualsOperation:ColumnOperator
    {
        public override bool Operate(object obj1, object obj2)
        {
            if (obj1 == null)
            {
                obj1 = string.Empty;
            }
            if (obj2 == null)
            {
                obj2 = string.Empty;
            }
           return string.Equals(obj1.ToString().Trim(), obj2.ToString().Trim(), StringComparison.CurrentCultureIgnoreCase);
        }
    }

    [DisplayName("日期相等")]
    public class DateTimeEqualsOperation : ColumnOperator
    {
        public override bool Operate(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null) return false;
            if(obj1.GetType()!=typeof(DateTime) || obj2.GetType()!=typeof(DateTime))
            {
                throw new FoxOneException("Parameter_Must_Be_DateTime");
            }
            return obj1.ConvertTo<DateTime>().Date.Equals(obj2.ConvertTo<DateTime>().Date);
        }
    }
}