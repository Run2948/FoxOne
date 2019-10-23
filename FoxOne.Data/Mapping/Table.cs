using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FoxOne.Core;
using System.ComponentModel;

namespace FoxOne.Data.Mapping
{
    public class Table
    {
        private readonly IList<Column> _keys = new List<Column>();
        private readonly IList<Column> _columns = new List<Column>();

        public String Schema { get; internal set; }

        public String Name { get; internal set; }

        public void AddKey(Column column)
        {
            _keys.Add(column);
        }

        public void AddColumn(Column column)
        {
            _columns.Add(column);
        }

        public IList<Column> Keys
        {
            get { return _keys; }
        }

        public IList<Column> Columns
        {
            get { return _columns; }
        }

        public Column GetColumn(String name)
        {
            return _columns.SingleOrDefault(col => col.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class Column
    {
        public Column()
        {
            Showable = true;
            Editable = true;
            Searchable = true;
        }


        [DisplayName("字段名")]
        public string Name { get; internal set; }

        [DisplayName("注释")]
        public string Comment { get; internal set; }

        [DisplayName("类型")]
        public string Type { get; internal set; }

        [DisplayName("长度")]
        public string Length { get; internal set; }

        [DisplayName("构架")]
        public string Schema { get; internal set; }

        [DisplayName("表")]
        public string Table { get; internal set; }

        [DisplayName("主健")]
        public bool IsKey { get; internal set; }

        [DisplayName("可为空")]
        public bool IsNullable { get; internal set; }

        [DisplayName("排序")]
        public int Rank { get; set; }

        [DisplayName("自增长")]
        public bool IsAutoIncrement { get; internal set; }

        public bool Showable { get; internal set; }

        public bool Editable { get; internal set; }

        public bool Searchable { get; internal set; }

        public FastProperty Property { get; internal set; }
    }
}
