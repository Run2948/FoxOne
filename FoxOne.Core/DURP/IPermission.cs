using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IPermission:IDURP
    {
        /// <summary>
        /// URL地址或控件ID
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// 所属父级
        /// </summary>
        string ParentId { get; set; }

        /// <summary>
        /// 行为：可见，不可见，可用，不可用。
        /// </summary>
        string Behaviour { get; set; }

        string Icon { get; set; }

        IPermission Parent { get; }

        PermissionType Type { get; set; }

        IEnumerable<IPermission> Childrens { get; }
    }

    public enum PermissionType
    {
        Module,
        Page,
        Control,
        Rule
    }

}