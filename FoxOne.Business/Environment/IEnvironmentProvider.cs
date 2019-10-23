using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FoxOne.Business.Environment
{
    public interface IEnvironmentProvider
    {
        string Prefix { get;  }

        object Resolve(string name);
    }
}