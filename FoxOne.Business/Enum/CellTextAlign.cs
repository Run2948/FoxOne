using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    /// <summary>
    /// 水平对齐方式
    /// </summary>
    public enum CellTextAlign
    {
        /// <summary>
        /// 居左
        /// </summary>
        [Description("居左")]
        Left,

        /// <summary>
        /// 居中
        /// </summary>
        [Description("居中")]
        Center,

        /// <summary>
        /// 居右
        /// </summary>
        [Description("居右")]
        Right
    }
}
