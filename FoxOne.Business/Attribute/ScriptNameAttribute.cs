using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public class ScriptNameAttribute:Attribute
    {
        public string Name { get; set; }

        public ScriptNameAttribute(string name)
        {
            Name = name;
        }
    }
}
