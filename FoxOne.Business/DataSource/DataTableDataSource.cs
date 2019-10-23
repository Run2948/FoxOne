using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoxOne.Core;
using FoxOne.Data;
using System.ComponentModel;
namespace FoxOne.Business
{
    /// <summary>
    /// 数据表数据源
    /// </summary>
    [DisplayName("数据表数据源")]
    public class DataTableDataSource : ListDataSourceBase, ICascadeDataSource, IKeyValueDataSource,IFormService
    {
        [FunctionDataSource(typeof(AllTableDataSource))]
        [FormField(ControlType = ControlType.DropDownList)]
        [DisplayName("表名")]
        public string TableName { get; set; }

        public string DataTextField { get; set; }

        public string DataValueField { get; set; }

        public string ParentField { get; set; }


        protected override IEnumerable<IDictionary<string, object>> GetListInner()
        {
            string queryString = GetQueryString();
            return Dao.Get().QueryDictionaries(queryString);
        }

        /*public override IEnumerable<IDictionary<string, object>> GetList(int pageIndex, int pageSize, out int recordCount)
        {
            string queryString = GetQueryString();
            return Dao.Get().PageQueryDictionariesByPage(queryString, pageIndex, pageSize, SortExpression, out recordCount, Parameter);
        }*/

        private string GetQueryString()
        {
            string sql = "SELECT * FROM {0}".FormatTo(TableName);
            if (!SortExpression.IsNullOrEmpty())
            {
                if (SortExpression.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    sql += " ORDER BY {0}".FormatTo(SortExpression);
                }
                else
                {
                    sql += SortExpression;
                }
            }
            return sql ;
        }

        public IEnumerable<TreeNode> SelectItems()
        {
            if (DataTextField.IsNullOrEmpty() || DataValueField.IsNullOrEmpty() || ParentField.IsNullOrEmpty())
            {
                throw new FoxOneException("必须设置DataTextField、DataValueField和ParentField属性");
            }
            var returnValue = new List<TreeNode>();
            var dicts = GetList();
            foreach (var dict in dicts)
            {
                returnValue.Add(new TreeNode()
                {
                    Text = dict[DataTextField].ToString(),
                    Value = dict[DataValueField].ToString(),
                    ParentId = (dict.Keys.Contains(ParentField) && dict[ParentField] != null) ? dict[ParentField].ToString() : string.Empty
                });
            }
            return returnValue;
        }

        private IEnumerable<TreeNode> _items;
        private IEnumerable<TreeNode> Items
        {
            get
            {
                return _items ?? (_items = SelectItems());
            }
        }

        public object Converter(string columnName, object columnValue, IDictionary<string, object> rowData)
        {
            if (!Items.IsNullOrEmpty())
            {
                var d = Items.FirstOrDefault(o => o.Value.Equals(columnValue.ToString(), StringComparison.CurrentCultureIgnoreCase));
                if (d!=null)
                {
                    return d.Text;
                }
            }
            return columnValue;
        }

        public int Insert(IDictionary<string, object> data)
        {
            return Dao.Get().Insert(TableName, data);
        }

        public int Update(string key, IDictionary<string, object> data)
        {
            return Dao.Get().Update(TableName, data);
        }

        public IDictionary<string, object> Get(string key)
        {
            return Dao.Get().Get(TableName, (object)key);
        }

        public int Delete(string key)
        {
            return Dao.Get().Delete(TableName, key);
        }
    }
}
