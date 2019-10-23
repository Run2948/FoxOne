using System;

namespace FoxOne.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,AllowMultiple = false,Inherited = true)]
    public class SchemaAttribute : NamedAttribute
    {
        public SchemaAttribute(string name)
            : base(name)
        {

        }
    }
}