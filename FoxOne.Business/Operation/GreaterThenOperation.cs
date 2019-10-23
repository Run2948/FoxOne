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
    [DisplayName("大于")]
    public class GreaterThenOperation:ColumnOperator
    {
        public override bool Operate(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null) return false;
            if (string.IsNullOrEmpty(obj1.ToString()) || string.IsNullOrEmpty(obj2.ToString())) return false;
            int i1 = 0, i2 = 0;
            if (obj1 is DateTime && obj2 is DateTime)
            {
                return CompareDateTime((DateTime)obj1, (DateTime)obj2);
            }
            else if ((obj1 is int && obj2 is int) || (obj1 is Enum && obj2 is Enum))
            {
                i1 = (int)obj1;
                i2 = (int)obj2;
            }
            else
            {
                i1 = int.Parse(obj1.ToString());
                i2 = int.Parse(obj2.ToString());
            }
            return Compare(i1, i2);
        }

        protected virtual bool Compare(int i1, int i2)
        {
            return i1 > i2;
        }

        protected virtual bool CompareDateTime(DateTime dt1, DateTime dt2)
        {
            return DateTime.Compare(dt1, dt2) > 0;
        }
    }
}