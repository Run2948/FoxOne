using System;
using System.Data;

namespace FoxOne.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : NamedAttribute
    {

        public ColumnAttribute()
            : base(null)
        {
            IsDataField = true;
            Insert = true;
            Update = true;
            Showable = true;
            Editable = true;
            Searchable = false;
        }

        public ColumnAttribute(string name)
            : base(name)
        {
            IsDataField = true;
            Insert = true;
            Update = true;
            Showable = true;
            Editable = true;
            Searchable = false;
        }

        public bool Insert
        {
            get;
            set;
        }

        public bool Update
        {
            get;
            set;
        }

        public bool Showable { get; set; }

        public bool Searchable { get; set; }

        public bool Editable { get; set; }

        public string DataType { get; set; }

        public string Length { get; set; }

        public bool IsDataField { get; set; }

        public bool IsAutoIncrement { get; set; }
    }
}