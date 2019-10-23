using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Controls
{
    public class Literal:PageControlBase
    {
        [FormField(ControlType=ControlType.TextArea)]
        public string Html { get; set; }

        public override string RenderContent()
        {
            return Html;
        }
    }
}
