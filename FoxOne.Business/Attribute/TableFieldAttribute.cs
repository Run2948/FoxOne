using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public class TableFieldAttribute:Attribute
    {
        public TableFieldAttribute()
        {
            IsShow = true;
        }
        public int ShowLength { get; set; }

        public string ColumnWidth { get; set; }

        public string DefaultValue { get; set; }

        public CellTextAlign TextAlign { get; set; }

        public string DataFormatString { get; set; }

        public string DataFormatFields { get; set; }

        public string ReferenceField { get; set; }

        public bool Sortable { get; set; }

        public bool IsShow { get; set; }

        public bool HtmlEncode { get; set; }
    }
}
