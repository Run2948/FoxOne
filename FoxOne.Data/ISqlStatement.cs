using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Data
{
    public interface ISqlStatement
    {
        bool IsQuery { get; }

        string Text  { get; }

        string Connection { get; set; }

        ISqlCommand CreateCommand(IDaoProvider provider, object parameters);
    }
}