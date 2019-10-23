using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{

    public interface IDURP:IEntity,ISortable,ILastUpdateTime
    {
        string Name { get; set; }

        string Alias { get; set; }

        string Code { get; set; }

        string Status { get; set; }

        IDictionary<string, object> Properties { get; }
    }
}
