/*********************************************************
 * 作　　者：刘海峰
 * 联系邮箱：mailTo:liuhf@foxone.net
 * 创建时间：2014/6/8 17:48:43
 * 描述说明：
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.ComponentModel;

namespace FoxOne.Business
{
    [DisplayName("分隔包含")]
    public class ContainOperation:ColumnOperator
    {
        public override bool Operate(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null) return false;
            char[] chars = new char[] { ',', '|', ';' };
            return obj1.ToString().Split(chars, StringSplitOptions.RemoveEmptyEntries).Contains(obj2.ToString(), StringComparer.Create(Thread.CurrentThread.CurrentCulture, true));
        }
    }


    [DisplayName("分隔包含于")]
    public class BeContainOperation:ContainOperation
    {
        public override bool Operate(object obj1, object obj2)
        {
            return base.Operate(obj2, obj1);
        }
    }
}