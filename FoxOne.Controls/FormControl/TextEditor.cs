using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Controls
{

    /// <summary>
    /// 文本编辑器
    /// </summary>
    [DisplayName("文本编辑器")]
    public class TextEditor : TextArea
    {
        public TextEditor()
        {
            CssClass = "form-control xheditor";
        }
        internal override void AddAttributes()
        {
            base.AddAttributes();
            Attributes["TextEditor"] = "TextEditor";
        }
    }
}
