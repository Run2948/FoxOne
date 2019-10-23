using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public interface IEntity   
    {
        string Id { get; set; }

        int RentId { get; set; }
    }

    public interface ISortable
    {
        int Rank { get; set; }
    }

    public interface ILastUpdateTime
    {
        DateTime LastUpdateTime { get; set; }
    }
}
