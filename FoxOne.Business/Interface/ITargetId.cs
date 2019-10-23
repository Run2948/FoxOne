using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public interface ITargetId
    {
        string TargetControlId { get; set; }

        void SetTarget(IList<IControl> components);
    }
}
