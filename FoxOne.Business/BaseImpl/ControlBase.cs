using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace FoxOne.Business
{
    public abstract class ControlBase:IControl
    {
        /// <summary>
        /// 控件ID（每页唯一）
        /// </summary>
        [Description("（每页唯一）")]
        [Validator("required")]
        public string Id
        {
            get;
            set;
        }

        [ScriptIgnore]
        [FormField(Editable = false)]
        public string PageId
        {
            get;
            set;
        }

        [ScriptIgnore]
        [FormField(Editable = false)]
        public string ParentId
        {
            get;
            set;
        }

        [ScriptIgnore]
        [FormField(Editable = false)]
        public string TargetId
        {
            get;
            set;
        }
    }
}
