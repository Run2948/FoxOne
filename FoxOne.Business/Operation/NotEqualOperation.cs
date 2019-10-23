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
    [DisplayName("不等于")]
    public class NotEqualOperation:EqualsOperation
    {

        public override bool Operate(object obj1, object obj2)
        {
            return !base.Operate(obj1, obj2);
        }
    }
}