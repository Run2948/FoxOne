using System;

namespace FoxOne.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,AllowMultiple = false,Inherited = true)]
    public class TablePrefixAttribute : NamedAttribute
    {
        public TablePrefixAttribute(string name)
            : base(name)
        {

        }
    }
}