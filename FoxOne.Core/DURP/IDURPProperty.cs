using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IDURPProperty : IEntity
    {
        string Name { get; set; }

        string Value { get; set; }

        string Type { get; set; }
    }
}
