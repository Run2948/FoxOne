using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public interface IAttributeAccessor
    {
        IDictionary<string, string> Attributes { get;}
    }
}
