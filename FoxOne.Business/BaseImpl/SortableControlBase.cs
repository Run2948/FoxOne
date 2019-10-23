using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public abstract class SortableControlBase : ControlBase, ISortableControl
    {
        public int Rank
        {
            get;
            set;
        }
    }
}
