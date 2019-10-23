using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FoxOne.Business
{
    [ModelBinder(typeof(ControlModelBinder))]
    public interface IControl
    {
        string Id { get; set; }

        string PageId { get; set; }

        string ParentId { get; set; }

        string TargetId { get; set; }
    }
}
