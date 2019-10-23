using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IExtProperty
    {
        IDictionary<string, object> Properties { get; }

        void SetProperty();
    }
}
