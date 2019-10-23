using FoxOne.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FoxOne.Core;
namespace FoxOne.Business
{
    /// <summary>
    /// SqlId数据源
    /// </summary>
    [DisplayName("SqlId列表数据源")]
    public class SqlIdListDataSource : ListDataSourceBase
    {
        [FunctionDataSource(typeof(AllSqlIdDataSource))]
        [FormField(ControlType=ControlType.DropDownList)]
        public string SqlId { get; set; }

        public override IEnumerable<IDictionary<string, object>> GetList()
        {
            return Dao.Get().QueryDictionaries(SqlId, Parameter);
        }

        public override IEnumerable<IDictionary<string, object>> GetList(int pageIndex, int pageSize, out int recordCount)
        {
            return Dao.Get().PageQueryDictionariesByPage(SqlId, pageIndex, pageSize, SortExpression, out recordCount, Parameter);
        }

        private IEnumerable<IDictionary<string, object>> _items;
        private IEnumerable<IDictionary<string, object>> Items
        {
            get
            {
                return _items ?? (_items = GetList());
            }
        }
    }
}
