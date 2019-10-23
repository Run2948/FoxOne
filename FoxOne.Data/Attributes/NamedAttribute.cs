using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Data.Attributes
{
    public abstract class NamedAttribute : Attribute
    {
        protected NamedAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

    }
}
