using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Globalization;
using FoxOne.Core;
using FoxOne.Business;
using System.IO;
namespace FoxOne.Controls
{
    public class ExcelHelper
    {
        private HSSFWorkbook workbook;

        private void GetLeafColumn(List<TableColumn> result, TableColumn column, ref int rank, ref int totalDeep)
        {
            if (column.Children.IsNullOrEmpty())
            {
                result.Add(column);
                column.Rank = rank++;
            }
            else
            {
                totalDeep += 1;
                foreach (var c in column.Children)
                {
                    GetLeafColumn(result, c, ref rank, ref totalDeep);
                }
            }
        }

        public HSSFWorkbook ExportToExcel(Table table, int freezeColumn = 0, int rowSpanColumnIndex = 0, string fileName = null, int ignoreRow = 0)
        {
            table.AllowPaging = false;
            var data = table.GetData();
            if (table.AutoGenerateColum)
            {
                table.GenerateTableColumn(data);
            }
            if (table.Columns.IsNullOrEmpty()) return new HSSFWorkbook();
            var tempFields = table.Columns.OrderBy(o => o.Rank).ToList();
            var fields = new List<TableColumn>();
            var tempRank = 1;
            int totalDeep = 1;
            int tempDeep = 1;
            foreach (var f in tempFields)
            {
                GetLeafColumn(fields, f, ref tempRank, ref tempDeep);
                if (tempDeep > totalDeep)
                {
                    totalDeep = tempDeep;
                }
                tempDeep = 1;
            }

            if (!fileName.IsNullOrEmpty())
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                workbook = new HSSFWorkbook(fs);
            }
            else
            {
                workbook = new HSSFWorkbook();
            }
            ISheet sheet = null;
            if (!fileName.IsNullOrEmpty())
            {
                sheet = workbook.GetSheetAt(0);
            }
            else
            {
                sheet = workbook.CreateSheet();
            }
            sheet.DefaultColumnWidth = 20;
            sheet.DefaultRowHeightInPoints = 25;
            int cellIndex = 0, rowIndex = totalDeep;
            if (!fileName.IsNullOrEmpty())
            {
                rowIndex = ignoreRow;
            }
            else
            {
                foreach (var field in tempFields)
                {
                    CreateHeader(field, sheet, totalDeep, 0, cellIndex);
                    cellIndex += GetLength(field);
                }
            }
            var newStartNow = rowIndex;
            foreach (var d in data)
            {
                var headerRow = sheet.CreateRow(rowIndex++);
                headerRow.HeightInPoints = 25;
                cellIndex = 0;
                foreach (var field in fields)
                {
                    field.RowData = d;
                    object tempDataValue = field.GetValue();
                    if (tempDataValue is CustomTd)
                    {
                        tempDataValue = (tempDataValue as CustomTd).Value;
                    }
                    var cell = headerRow.CreateCell(cellIndex++);
                    cell.SetCellValue(tempDataValue == null ? "" : tempDataValue.ToString().StripHTML());
                    cell.SetCellType(NPOI.SS.UserModel.CellType.STRING);
                    cell.CellStyle = GetCellStyle();
                }
            }
            if (rowSpanColumnIndex > 0)
            {
                for (cellIndex = 0; cellIndex < rowSpanColumnIndex; cellIndex++)
                {
                    MergeRow(sheet, newStartNow, cellIndex);
                }
            }
            sheet.CreateFreezePane(freezeColumn, totalDeep);
            return workbook;
        }

        private static void MergeRow(ISheet sheet, int newStartNow, int cellIndex)
        {
            int tempStart = newStartNow;
            string originalValue = sheet.GetRow(newStartNow).GetCell(cellIndex).StringCellValue;
            int i = 0;
            for (i = newStartNow + 1; i < sheet.LastRowNum; i++)
            {
                var value = sheet.GetRow(i).GetCell(cellIndex).StringCellValue;
                if (!value.Equals(originalValue, StringComparison.OrdinalIgnoreCase))
                {
                    originalValue = value;
                    if ((i - 1) > tempStart)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(tempStart, i - 1, cellIndex, cellIndex));
                    }
                    tempStart = i;
                }
            }
            if ((i-1) > tempStart)
            {
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(tempStart, i, cellIndex, cellIndex));
            }
        }

        private int GetLength(TableColumn field)
        {
            int result = 0;
            if (field.Children.IsNullOrEmpty())
            {
                result = 1;
            }
            else
            {
                foreach (var f in field.Children)
                {
                    result += GetLength(f);
                }
            }
            return result;
        }

        private void CreateHeader(TableColumn field, ISheet sheet, int totalDeep, int currentDeep, int cellIndex)
        {
            int length = GetLength(field);
            IRow headerRow = sheet.GetRow(currentDeep);
            if (headerRow == null)
            {
                headerRow = sheet.CreateRow(currentDeep);
            }
            headerRow.HeightInPoints = 30;
            int columnWidth = 0;
            string tempColumnWidth = string.Empty;
            for (int j = cellIndex; j < (cellIndex + length); j++)
            {
                var cell = headerRow.CreateCell(j);
                cell.SetCellValue(field.ColumnName.StripHTML());
                cell.SetCellType(NPOI.SS.UserModel.CellType.STRING);
                cell.CellStyle = GetHeaderStyle();
                if (!field.ColumnWidth.IsNullOrEmpty())
                {
                    tempColumnWidth = field.ColumnWidth;
                    if (field.ColumnWidth.EndsWith("px"))
                    {
                        tempColumnWidth = field.ColumnWidth.Replace("px", "");
                    }
                    if (int.TryParse(tempColumnWidth, out columnWidth))
                    {
                        sheet.SetColumnWidth(j, columnWidth * 35);
                    }
                }
            }
            if (field.Children.IsNullOrEmpty())
            {
                if (totalDeep - 1 > currentDeep)
                {
                    for (int k = currentDeep + 1; k < totalDeep; k++)
                    {
                        IRow tempRow = sheet.GetRow(k);
                        if (tempRow == null)
                        {
                            tempRow = sheet.CreateRow(k);
                        }
                        var tempCell = tempRow.CreateCell(cellIndex);
                        tempCell.SetCellValue(field.ColumnName.StripHTML());
                        tempCell.SetCellType(NPOI.SS.UserModel.CellType.STRING);
                        tempCell.CellStyle = GetHeaderStyle();
                    }
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(currentDeep, totalDeep - 1, cellIndex, cellIndex));
                }
            }
            else
            {
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(currentDeep, currentDeep, cellIndex, cellIndex + length - 1));
                currentDeep += 1;
                foreach (var f in field.Children)
                {
                    CreateHeader(f, sheet, totalDeep, currentDeep, cellIndex);
                    cellIndex += GetLength(f);
                }
            }
        }

        private ICellStyle GetHeaderStyle()
        {
            ICellStyle headerStyle = workbook.CreateCellStyle();
            headerStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
            headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_GREEN.index;
            headerStyle.Alignment = HorizontalAlignment.CENTER;
            SetBorder(headerStyle);
            var headerFont = workbook.CreateFont();
            headerFont.Color = NPOI.HSSF.Util.HSSFColor.BLACK.index;
            headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;
            headerFont.FontHeight = 13 * 13;
            headerStyle.SetFont(headerFont);
            return headerStyle;
        }

        private ICellStyle cellStyle;
        private ICellStyle GetCellStyle()
        {
            if (cellStyle == null)
            {
                cellStyle = workbook.CreateCellStyle();
                cellStyle.Alignment = HorizontalAlignment.LEFT;
                SetBorder(cellStyle);
            }
            return cellStyle;
        }

        private void SetBorder(ICellStyle cellStyle)
        {
            cellStyle.WrapText = true;
            cellStyle.VerticalAlignment = VerticalAlignment.CENTER;
            cellStyle.BorderBottom = BorderStyle.THIN;
            cellStyle.BorderLeft = BorderStyle.THIN;
            cellStyle.BorderRight = BorderStyle.THIN;
            cellStyle.BorderTop = BorderStyle.THIN;
            cellStyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
            cellStyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
            cellStyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
            cellStyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.BLACK.index;
        }
    }
}
