using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FoxOne.Core;
using System.Globalization;
using System.ComponentModel;
using System.Web;
using System.Web.Script.Serialization;
using System.Threading;
using FoxOne.Data.Mapping;
using FoxOne.Business.Security;
namespace FoxOne.Controls
{
    /// <summary> 
    /// 表格组件
    /// </summary>
    [DisplayName("表格组件")]
    public class Table : ListControlBase, IAuthorityComponent
    {
        public Table()
        {
            Columns = new List<TableColumn>();
            Buttons = new List<TableButton>();
            AllowPaging = true;
            PagerPosition = PagerPosition.Bottom;
            Pager = new Pager();
            AutoGenerateColum = false;
            CssClass = "data-list";
            TableCssClass = "table table-bordered table-striped table-hover";
            AutoHeight = true;
        }

        /// <summary>
        /// 自动生成列
        /// </summary>
        [DisplayName("自动生成列")]
        [Description("该属性指示是否自动显示数据源返回的结果")]
        public bool AutoGenerateColum { get; set; }

        [DisplayName("只显示返回列")]
        [Description("该属性指示是否只显示从数据源中返回的列，而不是显示所有已定义的列")]
        public bool ShowExistColumn { get; set; }

        /// <summary>
        /// 表格列
        /// </summary>
        [DisplayName("表格列")]
        public IList<TableColumn> Columns { get; set; }

        /// <summary>
        /// 表格行按钮
        /// </summary>
        [DisplayName("表格行按钮")]
        public IList<TableButton> Buttons { get; set; }

        /// <summary>
        /// 显示多选框
        /// </summary>
        [DisplayName("显示多选框")]
        public bool ShowCheckBox { get; set; }

        /// <summary>
        /// 显示序号
        /// </summary>
        [DisplayName("显示序号")]
        public bool ShowIndex { get; set; }

        /// <summary>
        /// 表格行主键字段
        /// </summary>
        [DisplayName("表格行主键字段")]
        public string KeyFieldName { get; set; }

        public string GroupByField { get; set; }

        [DisplayName("新增页面URL")]
        public string InsertUrl { get; set; }

        [DisplayName("编辑页面URL")]
        public string EditUrl { get; set; }

        [DisplayName("删除操作URL")]
        public string DeleteUrl { get; set; }


        public string TableCssClass { get; set; }

        public override string Render()
        {
            Attributes["data-inserturl"] = InsertUrl;
            Attributes["data-editurl"] = EditUrl;
            Attributes["data-deleteurl"] = DeleteUrl;
            return base.Render();
        }

        /// <summary>
        /// 是否生成操作列
        /// </summary>
        private bool RenderOperationColumn
        {
            get
            {
                return (!Buttons.IsNullOrEmpty() && Buttons.Count(o => o.TableButtonType == TableButtonType.TableRow && o.Visiable == true) > 0);
            }
        }

        private void HideNotExistColumn(IList<TableColumn> columns, string[] keys)
        {
            foreach (var column in columns)
            {
                if (!keys.Contains(column.FieldName))
                {
                    if (column.Children.Count == 0)
                    {
                        column.Visiable = false;
                    }
                    else
                    {
                        HideNotExistColumn(column.Children, keys);
                    }
                }
            }
        }

        public override string RenderContent()
        {
            var entities = GetData();
            if (entities.IsNullOrEmpty())
            {
                return RenderEmptyTable();
            }
            string[] keys = GetKeys(entities);
            if (AutoGenerateColum)
            {
                GenerateTableColumn(keys);
            }
            if (ShowExistColumn)
            {
                HideNotExistColumn(Columns, keys);
            }
            if (Columns.IsNullOrEmpty())
            {
                throw new ArgumentOutOfRangeException("Columns不能为空！");
            }
            TagBuilder table = new TagBuilder("table");
            table.AddCssClass(TableCssClass);
            var headerLevelConfig = new Dictionary<string, List<TableColumn>>();
            int headerDeep = 1;
            List<TableColumn> secondLevelHeader = null, thirdLevelHeader = null;
            var headerFinalField = new List<TableColumn>();
            bool renderOperationColumn = RenderOperationColumn;
            var columns = Columns.Where(o => o.Visiable == true).OrderBy(o => o.Rank).ToList();
            headerLevelConfig.Add("firstLevel", columns.ToList<TableColumn>());
            int tempRank = 1;
            foreach (var column in columns)
            {
                column.Rank = tempRank++;
                if (!column.Children.IsNullOrEmpty())
                {
                    column.Colspan = column.Children.Count;
                    headerDeep = 2;
                    if (secondLevelHeader == null)
                    {
                        secondLevelHeader = new List<TableColumn>();
                    }
                    secondLevelHeader.AddRange(column.Children);
                    foreach (var childField in column.Children.OrderBy(o => o.Rank))
                    {
                        childField.Rank = tempRank++;
                        if (!childField.Children.IsNullOrEmpty())
                        {
                            childField.Colspan = childField.Children.Count;
                            column.Colspan += childField.Children.Count;
                            column.Colspan--;
                            headerDeep = 3;
                            if (thirdLevelHeader == null)
                            {
                                thirdLevelHeader = new List<TableColumn>();
                            }
                            thirdLevelHeader.AddRange(childField.Children);
                            headerFinalField.AddRange(childField.Children);
                            childField.Children.ForEach(o => { o.Rank = tempRank++; });
                        }
                        else
                        {
                            headerFinalField.Add(childField);
                        }
                    }
                }
                else
                {
                    headerFinalField.Add(column);
                }
            }
            if (secondLevelHeader != null)
            {
                headerLevelConfig.Add("secondLevel", secondLevelHeader);
            }
            if (thirdLevelHeader != null)
            {
                headerLevelConfig.Add("thirdLevel", thirdLevelHeader);
            }
            TagBuilder theader = new TagBuilder("thead");
            bool renderIndexAndCheckbox = false;
            TagBuilder th = null;
            ///在这个地方弄表头
            foreach (var key in headerLevelConfig.Keys)
            {
                TagBuilder tr = new TagBuilder("tr");
                if (!renderIndexAndCheckbox)
                {
                    if (ShowCheckBox)
                    {
                        th = new TagBuilder("th");
                        th.Attributes["rowspan"] = headerDeep.ToString();
                        th.Attributes.Add("style", "width:50px");
                        th.InnerHtml = "<input type=\"checkbox\" checkAll=\"checkAll\" />";
                        tr.InnerHtml += th.ToString();
                    }
                    if (ShowIndex)
                    {
                        th = new TagBuilder("th");
                        th.Attributes["rowspan"] = headerDeep.ToString();
                        th.Attributes.Add("style", "width:50px");
                        th.InnerHtml = "序号";
                        tr.InnerHtml += th.ToString();
                    }
                    renderIndexAndCheckbox = true;
                }
                foreach (var field in headerLevelConfig[key].OrderBy(o => o.Rank))
                {
                    tr.InnerHtml += field.RenderHeader(headerDeep);
                }
                if (renderOperationColumn)
                {
                    th = new TagBuilder("th");
                    th.Attributes["rowspan"] = headerDeep.ToString();
                    th.InnerHtml = "操作";
                    tr.InnerHtml += th.ToString();
                    renderOperationColumn = false;
                }
                headerDeep--;
                theader.InnerHtml += ("\n" + tr.ToString() + "\n");
            }
            table.InnerHtml = "\n" + theader.ToString() + "\n";

            WriteTableBody(entities, table, headerFinalField);
            StringBuilder result = new StringBuilder();
            if (AllowPaging)
            {
                if (Pager.TargetControlId.IsNullOrEmpty())
                {
                    Pager.TargetControlId = Id;
                }
                if (PagerPosition == PagerPosition.Top || PagerPosition == PagerPosition.Both)
                {
                    result.AppendLine(Pager.Render());
                }
            }
            result.AppendLine(table.ToString());
            if (AllowPaging)
            {
                if (PagerPosition == PagerPosition.Bottom || PagerPosition == PagerPosition.Both)
                {
                    result.AppendLine(Pager.Render());
                }
            }
            return result.ToString();
        }

        private string RenderEmptyTable()
        {
            var table = new TagBuilder("table");
            table.AddCssClass("data-table");
            TagBuilder tr = new TagBuilder("tr");
            tr.InnerHtml = "<td style='padding:20px 0px;color:red;'>暂无相关数据</td>";
            table.InnerHtml = tr.ToString();
            return table.ToString();
        }

        private string[] GetKeys(IEnumerable<IDictionary<string, object>> entities)
        {
            string rowNum = "rownum";
            var firstKeys = entities.First().Keys;
            var keyList = firstKeys.ToList();
            if (keyList.Contains(rowNum))
            {
                keyList.Remove(rowNum);
            }
            string[] keys = keyList.ToArray();
            return keys;
        }

        private TableColumn GetColumn(IList<TableColumn> column, string fieldName)
        {
            TableColumn result = null;
            if (column.IsNullOrEmpty()) return result;
            foreach (var c in column)
            {
                if (c.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    result = c;
                    break;
                }
                result = GetColumn(c.Children, fieldName);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        public void GenerateTableColumn(IEnumerable<IDictionary<string, object>> entities)
        {
            string[] keys = GetKeys(entities);
            GenerateTableColumn(keys);
        }

        public void GenerateTableColumn(string[] keys)
        {
            if (DataSource is IAutoGenerateColumn)
            {
                (DataSource as IAutoGenerateColumn).GenerateColumn(Columns, keys);
            }
            else
            {
                int i = 1;
                foreach (var key in keys)
                {
                    TableColumn newField = GetColumn(Columns, key);
                    if (newField != null)
                    {
                        if (newField.Rank == 0)
                        {
                            newField.Rank = (i += 5);
                        }
                    }
                    else
                    {
                        newField = new TableColumn()
                        {
                            Id = "column{0}".FormatTo(key),
                            ColumnName = key,
                            FieldName = key,
                            Rank = (i += 5)
                        };
                        Columns.Add(newField);
                    }
                }
            }
        }

        private void WriteTableBody(IEnumerable<IDictionary<string, object>> entities, TagBuilder table, IList<TableColumn> columns)
        {
            TagBuilder tbody = new TagBuilder("tbody");
            int index = 1;
            foreach (var entity in entities)
            {
                TagBuilder tr = new TagBuilder("tr");
                var dict = new Dictionary<string, object>();
                if (!KeyFieldName.IsNullOrEmpty() && entity.Keys.Contains(KeyFieldName, StringComparer.Create(Thread.CurrentThread.CurrentCulture, true)))
                {
                    tr.Attributes.Add("data-key", entity[KeyFieldName].ToString());
                }
                if (ShowCheckBox)
                {
                    var indexTd = new TagBuilder("td");
                    indexTd.InnerHtml = "<input type=\"checkbox\" />";
                    tr.InnerHtml += indexTd.ToString();
                }
                if (ShowIndex)
                {
                    var indexTd = new TagBuilder("td");
                    if (entity.Keys.Contains("rownum"))
                    {
                        indexTd.InnerHtml = entity["rownum"].ToString();
                    }
                    else
                    {
                        indexTd.InnerHtml = index.ToString();
                    }
                    tr.InnerHtml += indexTd.ToString();
                    index++;
                }


                foreach (var column in columns.OrderBy(o => o.Rank))
                {
                    column.RowData = entity;
                    tr.InnerHtml += column.Render();
                }
                if (RenderOperationColumn)
                {
                    var buttonTd = new TagBuilder("td");
                    foreach (var button in Buttons.Where(o => o.TableButtonType == TableButtonType.TableRow && o.Visiable == true).OrderBy(o=>o.Rank))
                    {
                        button.RowData = entity;
                        buttonTd.InnerHtml += "\n" + button.Render() + "\n";
                    }
                    tr.InnerHtml += "\n" + buttonTd.ToString() + "\n";
                }
                tbody.InnerHtml += "\n" + tr.ToString() + "\n";
            }
            table.InnerHtml += tbody.ToString();
        }

        public void Authority(IDictionary<string, UISecurityBehaviour> behaviour)
        {
            foreach (var column in Columns)
            {
                if (behaviour.Keys.Contains(column.Id))
                {
                    column.Visiable = !behaviour[column.Id].IsInvisible;
                }
            }
            foreach (var button in Buttons)
            {
                if (behaviour.Keys.Contains(button.Id))
                {
                    button.Visiable = !behaviour[button.Id].IsInvisible;
                }
            }
        }
    }





    public enum TableButtonType
    {
        /// <summary>
        /// 表格行
        /// </summary>
        [Description("表格行")]
        TableRow,

        /// <summary>
        /// 表格行右键
        /// </summary>
        [Description("表格行右键")]
        TableRowContextMenu
    }

    public enum TableButtonTarget
    {
        Blank,
        Parent,
        Self,
        Top
    }

}
