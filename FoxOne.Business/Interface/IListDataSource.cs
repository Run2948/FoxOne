using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Business
{
    public interface IListDataSource : IControl
    {
        FoxOneDictionary<string, object> Parameter { get; set; }

        string SortExpression { get; set; }

        IEnumerable<IDictionary<string, object>> GetList();

        IEnumerable<IDictionary<string, object>> GetList(int pageIndex, int pageSize, out int recordCount);

    }
}
