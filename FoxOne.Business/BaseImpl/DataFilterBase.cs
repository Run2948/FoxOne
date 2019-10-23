using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
namespace FoxOne.Business
{

    public abstract class DataFilterBase : SortableControlBase, IDataFilter
    {
        public abstract bool Filter(IDictionary<string, object> data);
    }
}
