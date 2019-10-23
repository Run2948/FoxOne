using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Data.Mapping;
namespace FoxOne.Controls
{
    public static class ControlDefaultSetting
    {
        public static IList<TableButton> GetDefaultTableButton()
        {
            var result = new List<TableButton>();
            result.Add(new TableButton() { CssClass = "btn btn-default btn-sm", Id = "btnEdit", Name = "编辑", TableButtonType = TableButtonType.TableRow });
            result.Add(new TableButton() { CssClass = "btn btn-default btn-sm", Id = "btnDelete", Name = "删除", TableButtonType = TableButtonType.TableRow });
            return result;
        }

        public static IList<Button> GetDefaultToolbarButton()
        {
            var result = new List<Button>();
            result.Add(new Button() { CssClass = "btn btn-primary", Id = "btnInsert", Label = "新增", ButtonType = ButtonType.Button });
            result.Add(new Button() { CssClass = "btn btn-danger", Id = "btnBatchDelete", Label = "批量删除", ButtonType = ButtonType.Button });
            return result;
        }

        public static IList<Button> GetDefaultFormButton()
        {
            var result = new List<Button>();
            result.Add(new Button() { CssClass = "btn btn-success btn-big", Id = "btnSubmit", Label = "保存", ButtonType = ButtonType.Submit });
            result.Add(new Button() { CssClass = "btn btn-danger btn-big", Id = "btnReset", Label = "重置", ButtonType = ButtonType.Reset });
            return result;
        }

        public static IList<Button> GetDefaultSearchButton()
        {
            var result = new List<Button>();
            result.Add(new Button() { CssClass = "btn btn-primary", Id = "btnSearchSubmit", Label = "查询", ButtonType = ButtonType.Submit });
            result.Add(new Button() { CssClass = "btn btn-default", Id = "btnExportExcel", Label = "导出EXCEL", ButtonType = ButtonType.Button });
            return result;
        }

        public static FormControlBase GetFormControl(Column field)
        {
            FormControlBase result = null;
            switch (field.Type.ToLower())
            {
                case "datetime":
                    result = new DatePicker();
                    break;
                case "bit":
                    result = new DropDownList()
                    {
                        DataSource = GetDataSource(field) as IKeyValueDataSource
                    };
                    break;
                default:
                    result = new TextBox();
                    break;
            }
            result.Id = field.Name;
            result.Name = field.Name;
            result.Label = field.Comment;
            result.Rank = field.Rank;
            result.Enable = true;
            result.Visiable = true;
            return result;
        }

        public static TableColumn GetTableColumn(Column field)
        {
            var column = new TableColumn()
            {
                Id = "column{0}".FormatTo(field.Name),
                FieldName = field.Name,
                ColumnName = field.Comment,
                IsKey = field.IsKey,
                Sortable = true,
                Visiable = true,
                TextAlign = CellTextAlign.Center,
                ShowLength = 10,
                Rank = (field.Rank * 3)
            };
            column.ColumnConverter = GetDataSource(field);
            return column;
        }

        public static IFieldConverter GetDataSource(Column field)
        {
            IFieldConverter result = null;
            if (field.Type.ToLower() == "bit")
            {
                result = new EnumDataSource(typeof(YesOrNo));
            }
            if (field.Name.IndexOf("UserId") >= 0 || field.Name.IndexOf("Creator") >= 0)
            {
                result = new EntityDataSource() { EntityType=typeof(User) };
            }
            if (field.Name.IndexOf("DepartmentId") >= 0)
            {
                result = new EntityDataSource() {EntityType=typeof(Department) };
            }
            return result;
        }
    }
}
