
using FoxOne.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using FoxOne.Data.Attributes;
using System.Web.Mvc;
using FoxOne.Data.Sql;
using System.ComponentModel;
using System.Web;
using System.Threading;
using System.Web.Script.Serialization;
namespace FoxOne.Business
{
    /// <summary>
    /// 实体数据源
    /// </summary>
    [DisplayName("实体数据源")]
    public class EntityDataSource : ListDataSourceBase, ICascadeDataSource, IFormService, IKeyValueDataSource
    {
        public EntityDataSource()
        {
        }

        private string entityTypeFullName;
        private Type _entityType;

        [DisplayName("实体")]
        [TypeDataSource(typeof(IAutoCreateTable))]
        [FormField(ControlType = ControlType.DropDownList)]
        public string EntityTypeFullName
        {
            get
            {
                return entityTypeFullName;
            }
            set
            {
                entityTypeFullName = value;
                EntityType = TypeHelper.GetType(value);

            }
        }

        [ScriptIgnore]
        public Type EntityType
        {
            get
            {
                return _entityType;
            }
            set
            {
                _entityType = value;
            }
        }

        protected override IEnumerable<IDictionary<string, object>> GetListInner()
        {
            if (EntityType == null)
            {
                throw new FoxOneException("Parameter_Not_Found", "EntityType");
            }
            return Dao.Get().Select(EntityType).ToDictionary();
        }

        public IEnumerable<TreeNode> SelectItems()
        {
            IList<TreeNode> result = new List<TreeNode>();
            string TitleField = "Name";
            string ParentId = "ParentId";
            string ValueField = "Id";
            var items = GetListInner();
            if (items.IsNullOrEmpty())
            {
                return result;
            }
            if (!items.First().Keys.Contains(TitleField, StringComparer.OrdinalIgnoreCase))
            {
                TitleField = "Title";
            }
            string pid = string.Empty;
            foreach (var item in items)
            {
                pid = item.Keys.Contains(ParentId) ? item[ParentId] as string : string.Empty;
                result.Add(new TreeNode()
                {
                    Value = item[ValueField] as string,
                    Text = item[TitleField] as string,
                    ParentId = item.Keys.Contains(ParentId) ? item[ParentId] as string : string.Empty,
                    Open = pid.IsNullOrEmpty()
                });
            }
            return result;
        }

        public object Converter(string columnName, object columnValue, IDictionary<string, object> rowData)
        {
            if (columnValue == null) return columnValue;
            if (Items.IsNullOrEmpty())
            {
                return columnValue;
            }
            var i = Items.FirstOrDefault(o => o.Value.Equals(columnValue.ToString(), StringComparison.OrdinalIgnoreCase));
            if (i != null)
            {
                return i.Text;
            }
            var items = Items.Where(o => columnValue.ToString().Split(',').Contains(o.Value, StringComparer.OrdinalIgnoreCase));
            if (!items.IsNullOrEmpty())
            {
                return string.Join(",", items.Select(o => o.Text));
            }
            return columnValue;
        }

        private IEnumerable<TreeNode> _items;
        private IEnumerable<TreeNode> Items
        {
            get
            {
                return _items ?? (_items = SelectItems());
            }
        }

        public int Insert(IDictionary<string, object> data)
        {
            var item = data.ToEntity(EntityType);
            var result = (bool)InvokeMethod("Insert", item);
            if (result)
            {
                return 1;
            }
            return 0;
        }

        private object InvokeMethod(string methodName, object item)
        {
            try
            {
                var service = typeof(DBContext<>);
                service = service.MakeGenericType(EntityType);
                var result = service.InvokeMember(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod, null, null, new object[] { item });
                return result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public int Update(string key, IDictionary<string, object> data)
        {
            var item = data.ToEntity(EntityType);
            if (item is IEntity)
            {
                (item as IEntity).Id = key;
            }
            var result = (bool)InvokeMethod("Update", item);
            if (result)
            {
                return 1;
            }
            return 0;
        }

        public IDictionary<string, object> Get(string key)
        {
            return InvokeMethod("Get", key).ToDictionary();
        }

        public int Delete(string key)
        {
            var result = (bool)InvokeMethod("Delete", key);
            if (result)
            {
                return 1;
            }
            return 0;
        }
    }
}
