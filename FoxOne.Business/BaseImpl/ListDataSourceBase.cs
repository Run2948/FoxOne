using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
using System.Web.Script.Serialization;
using System.ComponentModel;
namespace FoxOne.Business
{
    public abstract class ListDataSourceBase : ControlBase, IListDataSource
    {
        public virtual FoxOneDictionary<string, object> Parameter
        {
            get;
            set;
        }

        public virtual string SortExpression
        {
            get;
            set;
        }

        [DisplayName("数据源过滤器")]
        public IDataFilter DataFilter
        {
            get;
            set;
        }


        [DisplayName("数据源列转换器")]
        public IList<IColumnConverter> ColumnConverters
        {
            get;
            set;
        }

        protected virtual void AppendDefaultConverter(IList<IColumnConverter> columnConverters)
        {

        }

        protected virtual IEnumerable<IDictionary<string, object>> GetListInner()
        {
            return null;
        }

        protected void Convert(IDictionary<string, object> data)
        {
            if (!ColumnConverters.IsNullOrEmpty())
            {
                foreach (var converter in ColumnConverters.OrderBy(o => o.Rank))
                {
                    foreach (var columnName in converter.ColumnName.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!data.ContainsKey(columnName)) continue;
                        converter.RowData = data;
                        switch (converter.ConverterType)
                        {
                            case ColumnConverterType.Append:
                                data[converter.AppendColumnName] = converter.Converter(data[columnName]);
                                break;
                            case ColumnConverterType.Replace:
                                data[columnName] = converter.Converter(data[columnName]);
                                break;
                            case ColumnConverterType.Rename:
                                data[converter.AppendColumnName] = data[columnName];
                                data.Remove(columnName);
                                break;
                            case ColumnConverterType.Remove:
                                data.Remove(columnName);
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
        }

        public virtual IEnumerable<IDictionary<string, object>> GetList()
        {
            var source = GetListInner();
            if (ColumnConverters.IsNullOrEmpty())
            {
                ColumnConverters = new List<IColumnConverter>();
            }
            AppendDefaultConverter(ColumnConverters);
            if (!source.IsNullOrEmpty() && (DataFilter != null || !ColumnConverters.IsNullOrEmpty()))
            {
                if (DataFilter != null && ColumnConverters.IsNullOrEmpty())
                {
                    source = source.Where(o => DataFilter.Filter(o));
                }
                else if (DataFilter == null && !ColumnConverters.IsNullOrEmpty())
                {
                    source = source.Where(o => { Convert(o); return true; });
                }
                else
                {
                    var result = new List<IDictionary<string, object>>();
                    source.ForEach(o =>
                    {
                        if (DataFilter.Filter(o))
                        {
                            Convert(o);
                            result.Add(o);
                        }
                    });
                    source = result;
                }
            }
            return source;
        }

        public virtual IEnumerable<IDictionary<string, object>> GetList(int pageIndex, int pageSize, out int recordCount)
        {
            var list = GetList();
            if (list.IsNullOrEmpty())
            {
                recordCount = 0;
                return null;
            }
            recordCount = list.Count();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
    }
}
