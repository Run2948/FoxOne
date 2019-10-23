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
    [DisplayName("小于")]
    public class LessThenOperation:GreaterThenOperation
    {
        protected override bool Compare(int i1, int i2)
        {
            return i1 < i2;
        }

        protected override bool CompareDateTime(DateTime dt1, DateTime dt2)
        {
            return DateTime.Compare(dt1, dt2) < 0;
        }
    }
}