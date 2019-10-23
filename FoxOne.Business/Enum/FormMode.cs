using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public enum FormMode
    {
        [Description("新增")]
        Insert,

        [Description("编辑")]
        Edit,

        [Description("查看")]
        View
    }
}
