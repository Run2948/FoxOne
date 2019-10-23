using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FoxOne.Core;
namespace FoxOne.Business
{
    [DisplayName("复合过滤器")]
    public class CompositeDataFilter:DataFilterBase
    {
        [DisplayName("子过滤器")]
        public IList<IDataFilter> DataFilters
        {
            get;
            set;
        }


        [DisplayName("过滤器运算类型")]
        public OperatorType OperatorType { get; set; }


        public override bool Filter(IDictionary<string, object> data)
        {
            bool result = (OperatorType == OperatorType.And);
            bool unResult = !result;
            if (!DataFilters.IsNullOrEmpty())
            {
                foreach (var filter in DataFilters.OrderBy(o => o.Rank))
                {
                    result = filter.Filter(data);
                    if (result == unResult)
                    {
                        break;
                    }
                }
            }
            return result;
        }
    }
}
