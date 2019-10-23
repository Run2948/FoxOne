using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public enum ControlType
    {
        None,

        /// <summary>
        /// 单行文本框
        /// </summary>
        TextBox,

        /// <summary>
        /// 多行文本框
        /// </summary>
        TextArea,

        /// <summary>
        /// 隐藏域
        /// </summary>
        HiddenField,

        /// <summary>
        /// 密码框
        /// </summary>
        Password,

        /// <summary>
        /// 日期点选框
        /// </summary>
        DatePicker,

        /// <summary>
        /// 下拉框（需要用到数据源）
        /// </summary>
        DropDownList,

        /// <summary>
        /// 多选框
        /// </summary>
        CheckBox,

        /// <summary>
        /// 多选框集
        /// </summary>
        CheckBoxList,

        /// <summary>
        /// 单选框
        /// </summary>
        RadioButton,

        /// <summary>
        /// 单选框集
        /// </summary>
        RadioButtonList,

        /// <summary>
        /// 文本编辑器
        /// </summary>
        TextEditor,

        /// <summary>
        /// 时间区间
        /// </summary>
        DateTimeRange,

        /// <summary>
        /// KeyValue点选框
        /// </summary>
        TextValueTextBox,

        /// <summary>
        /// 单点框
        /// </summary>
        CheckLabelList
    }
}
