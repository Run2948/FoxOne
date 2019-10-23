using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FoxOne.Data.Attributes;
using FoxOne.Core;

namespace FoxOne.Data.Mapping
{
    public class TableMapping
    {
        private readonly IList<Column> _insertColumns = new List<Column>();
        private readonly IList<Column> _updateColumns = new List<Column>();

        public TableMapping(Table table)
        {
            this.Table = table;
            Initialize();
        }

        public TableMapping(Type type, Table table)
        {
            this.Type = type;
            this.Table = table;
            Initialize();
        }

        public Type Type
        {
            get;
            internal set;
        }

        public Table Table
        {
            get;
            internal set;
        }

        public IList<Column> InsertColumns
        {
            get { return _insertColumns; }
        }

        public IList<Column> UpdateColumns
        {
            get { return _updateColumns; }
        }

        public FastProperty GetMappingProperty(string columnName)
        {
            Column column = Table.Columns.SingleOrDefault(col => col.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

            return (column == null || column.Property==null) ? null : column.Property;
        }

        private void Initialize()
        {
            PropertyInfo[] props = null;
            if (Type != null)
            {
                props = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }
            Table.Columns.ForEach(col =>
            {
                ColumnAttribute attr = null;

                FastProperty prop = null;
                if (!props.IsNullOrEmpty())
                {
                    foreach (PropertyInfo info in props)
                    {
                        attr = info.GetCustomAttribute<ColumnAttribute>(true);
                        if (null != attr && !string.IsNullOrEmpty(attr.Name))
                        {
                            if (col.Name.Equals(attr.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                prop = new FastProperty(attr.Name, info);
                                break;
                            }
                        }
                        else
                        {
                            if (col.Name.Equals(info.Name, StringComparison.OrdinalIgnoreCase) ||
                                    col.Name.Replace("_", "").Replace(" ", "").Equals(info.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                prop = new FastProperty(info.Name, info);
                                break;
                            }
                        }
                    }
                }
                if (null != prop)
                {
                    col.Property = prop;
                }
                if (null == attr || attr.Insert)
                {
                    _insertColumns.Add(col);
                }

                if (null == attr || attr.Update)
                {
                    _updateColumns.Add(col);
                }
            });
        }
    }
}