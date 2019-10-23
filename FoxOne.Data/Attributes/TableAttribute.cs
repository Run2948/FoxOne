using System;

namespace FoxOne.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,AllowMultiple = false,Inherited = true)]
    public class TableAttribute : NamedAttribute
    {
        public TableAttribute(string name) : base(name)
        {

        }
    }
}