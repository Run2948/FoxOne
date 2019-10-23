using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Controls
{

    /// <summary>
    /// 单选框
    /// </summary>
    [DisplayName("单选框")] 
    public class RadioButton:CheckBox
    {
        public RadioButton()
        {
            CssClass = "radio";
        }
        internal override void AddAttributes()
        {
            base.AddAttributes();
            Attributes["type"]= "radio";
        }
    }
}
