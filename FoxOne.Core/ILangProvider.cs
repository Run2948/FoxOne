using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface ILangProvider
    {
        string GetString(string name);
    }
}
