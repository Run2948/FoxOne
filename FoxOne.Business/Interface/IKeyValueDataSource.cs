using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FoxOne.Business
{
    public interface IKeyValueDataSource : IFieldConverter
    {
        IEnumerable<TreeNode> SelectItems();
    }
}
