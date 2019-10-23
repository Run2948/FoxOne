using FoxOne.Core;
using FoxOne.Data;
using FoxOne.Data.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace FoxOne.Business
{
    [Category("None")]
    [DisplayName("AllEnum")]
    public class AllEnumDataSource:KeyValueDataSourceBase
    {
        public override IEnumerable<TreeNode> SelectItems()
        {
            var result = new List<TreeNode>();
            var types = TypeHelper.GetAllEnum();
            foreach(var type in types)
            {
                result.Add(new TreeNode()
                {
                    Text = type.FullName,
                    Value = type.FullName
                });
            }
            return result;
        }
    }

    [Category("None")]
    [DisplayName("DictionaryCode")]
    public class DictionaryCodeDataSource:KeyValueDataSourceBase
    {
        public override IEnumerable<TreeNode> SelectItems()
        {
            return DBContext<DataDictionary>.Instance.Where(o => o.ParentId.IsNullOrEmpty()).Select(o => new TreeNode() { Text = o.Name, Value = o.Code }).ToList();
        }
    }

    [Category("None")]
    [DisplayName("AllTable")]
    public class AllTableDataSource:KeyValueDataSourceBase
    {
        public override IEnumerable<TreeNode> SelectItems()
        {
            return TableMapper.Tables[Dao.Get().ConnectionString].OrderBy(o=>o.Name).Select(o => new TreeNode() { Text = o.Name, Value = o.Name }).ToList();
        }
    }

    [Category("None")]
    [DisplayName("所有SqlId语句")]
    public class AllSqlIdDataSource : KeyValueDataSourceBase
    {
        public override IEnumerable<TreeNode> SelectItems()
        {
            var result = new List<TreeNode>();
            string key = string.Empty;
            foreach (var d in DaoFactory.GetSqlSource().Sqls)
            {
                key = d.Key.Trim();
                if (key.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                result.Add(new TreeNode()
                {
                    Text = key,
                    Value = key
                });

            }
            return result;
        }
    }

    [Category("None")]
    [DisplayName("类型数据源")]
    public class TypeDataSource : KeyValueDataSourceBase
    {
        public Type BaseType { get; set; }

        public override IEnumerable<TreeNode> SelectItems()
        {
            var types = BaseType.IsInterface ? TypeHelper.GetAllImpl(BaseType) : TypeHelper.GetAllSubType(BaseType);
            var result = new List<TreeNode>();
            string displayName = string.Empty;
            foreach (var type in types)
            {
                if (!type.IsAbstract)
                {
                    result.Add(new TreeNode() { Text = type.GetDisplayName(), Value = type.FullName });
                }
            }
            return result;
        }
    }

    [Category("None")]
    public class AllCRUDDataSource:KeyValueDataSourceBase
    {
        public override IEnumerable<TreeNode> SelectItems()
        {
            return DBContext<CRUDEntity>.Instance.Where(o => true).OrderBy(o => o.Id).Select(o => new TreeNode() { Text = o.Id, Value = o.Id }).ToList();
        }
    }
}
