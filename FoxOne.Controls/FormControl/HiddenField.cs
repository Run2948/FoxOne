using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Controls
{

    /// <summary>
    /// 隐藏域
    /// </summary>
    [DisplayName("隐藏域")]
    public class HiddenField : TextBox
    {
        internal override void AddAttributes()
        {
            base.AddAttributes();
            Attributes["type"] = "hidden";
        }
    }
}
