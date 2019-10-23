using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public abstract class DataSourceAttribute : Attribute
    {
        public abstract IControl GetDataSource();
    }

    public class EnumDataSourceAttribute : DataSourceAttribute
    {
        private Type EnumType { get; set; }
        public EnumDataSourceAttribute(Type type)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentOutOfRangeException("type");
            }
            EnumType = type;
        }

        public override IControl GetDataSource()
        {
            var result = new EnumDataSource();
            result.EnumType = EnumType;
            result.EnumValueType = EnumValueType.Code;
            return result;
        }
    }

    public class FunctionDataSourceAttribute : DataSourceAttribute
    {
        private Type Type { get; set; }

        public FunctionDataSourceAttribute(Type type)
        {
            Type = type;
        }
        public override IControl GetDataSource()
        {
            return Activator.CreateInstance(Type) as IControl;
        }
    }

    public class TypeDataSourceAttribute : DataSourceAttribute
    {
        private Type Type { get; set; }

        public TypeDataSourceAttribute(Type type)
        {
            Type = type;
        }
        public override IControl GetDataSource()
        {
            return new TypeDataSource() { BaseType = Type } as IKeyValueDataSource;
        }
    }
}
