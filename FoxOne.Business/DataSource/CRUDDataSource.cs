using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.ComponentModel;
using FoxOne.Data;
namespace FoxOne.Business
{
    [DisplayName("CRUD数据源")]
    public class CRUDDataSource : ListDataSourceBase, IFormService, IKeyValueDataSource, ICascadeDataSource
    {
        [FormField(ControlType = ControlType.DropDownList)]
        [FunctionDataSource(typeof(AllCRUDDataSource))]
        public string CRUDName { get; set; }

        private Dao dao { get { return Dao.Get(); } }

        private CRUDEntity _entity;
        private CRUDEntity Entity
        {
            get
            {
                if (!CRUDName.IsNullOrEmpty())
                {
                    _entity = DBContext<CRUDEntity>.Instance.Get(CRUDName);
                    if (_entity == null)
                    {
                        throw new FoxOneException("CRUDName不存在!");
                    }
                }
                return _entity;
            }
        }

        public virtual int Insert(IDictionary<string, object> data)
        {
            if (!data.IsNullOrEmpty())
            {
                if (!data.Keys.Contains(KeyFieldName))
                {
                    data[KeyFieldName] = Guid.NewGuid().ToString();
                }
                return dao.ExecuteNonQuery(Entity.InsertSQL, data);
            }
            return 0;
        }

        public virtual int Update(string key, IDictionary<string, object> data)
        {
            if (!data.IsNullOrEmpty())
            {
                if (!key.IsNullOrEmpty())
                {
                    data[Entity.PKName] = key;
                }
                return dao.ExecuteNonQuery(Entity.UpdateSQL, data);
            }
            return 0;
        }

        public virtual int Delete(string key)
        {
            var parameter = new Dictionary<string, object>();
            if (!key.IsNullOrEmpty())
            {
                parameter[Entity.PKName] = key;
                if (!Entity.ValueField.IsNullOrEmpty() && !Entity.ValueField.Equals(Entity.PKName, StringComparison.CurrentCultureIgnoreCase))
                {
                    parameter[Entity.ValueField] = key;
                }
            }
            return dao.ExecuteNonQuery(Entity.DeleteSQL, parameter);
        }

        public virtual IDictionary<string, object> Get(string key)
        {
            var parameter = new Dictionary<string, object>();
            if (!key.IsNullOrEmpty())
            {
                parameter[Entity.PKName] = key;
                if (!Entity.ValueField.IsNullOrEmpty() && !Entity.ValueField.Equals(Entity.PKName, StringComparison.CurrentCultureIgnoreCase))
                {
                    parameter[Entity.ValueField] = key;
                }
            }
            return dao.QueryDictionary(Entity.GetOneSQL, parameter);
        }

        public override IEnumerable<IDictionary<string, object>> GetList()
        {
            string sql = Entity.SelectSQL;
            if (sql.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) < 0)
            {
                if (SortExpression.IsNullOrEmpty())
                {
                    SortExpression = Entity.DefaultSortExpression;
                }
                if (SortExpression.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    SortExpression = " ORDER BY {0}".FormatTo(SortExpression);
                }
                sql = sql + SortExpression;
            }
            return dao.QueryDictionaries(sql, Parameter);
        }

        public override IEnumerable<IDictionary<string, object>> GetList(int pageIndex, int pageSize, out int recordCount)
        {
            recordCount = 0;
            if (SortExpression.IsNullOrEmpty())
            {
                SortExpression = Entity.DefaultSortExpression;
            }
            return dao.PageQueryDictionariesByPage(Entity.SelectSQL, pageIndex, pageSize, SortExpression, out recordCount, Parameter);
        }

        private IEnumerable<IDictionary<string, object>> _items;
        private IEnumerable<IDictionary<string, object>> Items
        {
            get
            {
                return _items ?? (_items = GetList());
            }
        }

        public object Converter(string columnName, object columnValue, IDictionary<string, object> rowData)
        {
            if (columnValue != null && !columnName.ToString().IsNullOrEmpty() && !Items.IsNullOrEmpty())
            {
                IDictionary<string, object> result = null;
                if (Items.First().ContainsKey(Entity.PKName))
                {
                    result = Items.FirstOrDefault(o => o[Entity.PKName].ToString().Equals(columnValue.ToString(), StringComparison.OrdinalIgnoreCase));
                }
                if (result.IsNullOrEmpty() && !Entity.PKName.Equals(Entity.ValueField, StringComparison.OrdinalIgnoreCase) && Items.First().ContainsKey(Entity.ValueField))
                {
                    result = Items.FirstOrDefault(o => o[Entity.ValueField].ToString().Equals(columnValue.ToString(), StringComparison.OrdinalIgnoreCase));
                }
                if (result.IsNullOrEmpty())
                {
                    result = Get(columnValue.ToString());
                }
                if (!result.IsNullOrEmpty())
                {
                    return result[Entity.TitleField];
                }
            }
            return string.Empty;
        }

        public string KeyFieldName
        {
            get
            {
                if (Entity != null)
                {
                    return Entity.PKName;
                }
                return string.Empty;
            }
        }

        public IEnumerable<TreeNode> SelectItems()
        {
            var returnValue = new List<TreeNode>();
            TreeNode node = null;
            foreach (var r in Items)
            {
                node = new TreeNode() { Text = r[Entity.TitleField].ToString(), Value = r[Entity.ValueField].ToString() };
                if (Entity.ParentField.IsNotNullOrEmpty() && r.Keys.Contains(Entity.ParentField))
                {
                    node.ParentId = r[Entity.ParentField].ToString();
                }
                returnValue.Add(node);
            }
            return returnValue;
        }
    }
}
