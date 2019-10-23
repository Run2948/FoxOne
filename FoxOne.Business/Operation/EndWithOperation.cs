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
    [DisplayName("以……结尾")]
    public class EndWithOperation:ColumnOperator
    {
        public override bool Operate(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null) return false;
            return obj1.ToString().EndsWith(obj2.ToString());
        }
    }
}