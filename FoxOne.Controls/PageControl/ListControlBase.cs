using FoxOne.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using FoxOne.Core;
using System.Threading;
namespace FoxOne.Controls
{
    public abstract class NoPagerListControlBase : PageControlBase, IListDataSourceControl
    {
        public override object Clone()
        {
            var result = base.Clone() as NoPagerListControlBase;
            if (result.DataSource != null)
            {
                result.DataSource.Parameter = null;
                result.DataSource.SortExpression = "";
            }
            return result;
        }

        /// <summary>
        /// 表格数据源
        /// </summary>
        [DisplayName("表格数据源")]
        public IListDataSource DataSource
        {
            get;
            set;
        }

        private void InitDataSourceParameter()
        {
            if (DataSource != null)
            {
                var param = DataSource.Parameter; 
                if (param == null)
                {
                    param = new FoxOneDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }
                foreach (var key in HttpContext.Current.Request.QueryString.AllKeys)
                {
                    if (!HttpContext.Current.Request.QueryString[key].IsNullOrEmpty())
                    {
                        param[key] = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString[key]);
                    }
                }
                foreach (var key in HttpContext.Current.Request.Form.AllKeys)
                {
                    if (!HttpContext.Current.Request.Form[key].IsNullOrEmpty())
                    {
                        param[key] = HttpUtility.UrlDecode(HttpContext.Current.Request.Form[key]);
                    }
                }
                DataSource.Parameter = param;
            }
        }

        protected virtual IEnumerable<IDictionary<string, object>> GetDataInner()
        {
            if (DataSource == null)
            {
                throw new FoxOneException("Need_DataSource",this.Id);
            }
            return DataSource.GetList();
        }

        public virtual IEnumerable<IDictionary<string, object>> GetData()
        {
            InitDataSourceParameter();
            var entities = GetDataInner();
            return entities;
        }
    }

    public abstract class ListControlBase : NoPagerListControlBase
    {

        /// <summary>
        /// 是否翻页
        /// </summary>
        [DisplayName("是否翻页")]
        public bool AllowPaging { get; set; }

        /// <summary>
        /// 翻页显示位置
        /// </summary>
        [DisplayName("翻页显示位置")]
        public PagerPosition PagerPosition { get; set; }

        /// <summary>
        /// 翻页配置
        /// </summary>
        [DisplayName("翻页配置")]
        public Pager Pager { get; set; }

        protected override IEnumerable<IDictionary<string, object>> GetDataInner()
        {
            if (DataSource == null)
            {
                throw new FoxOneException("Need_DataSource",this.Id);
            }
            int recordCount = 0;
            IEnumerable<IDictionary<string, object>> entities = null;
            if (AllowPaging)
            {
                string pageIndex=HttpContext.Current.Request[NamingCenter.PARAM_PAGE_INDEX];
                string pageSize = HttpContext.Current.Request[NamingCenter.PARAM_PAGE_SIZE];
                string sortExpression = HttpContext.Current.Request[NamingCenter.PARAM_SORT_EXPRESSION];
                if (!pageIndex.IsNullOrEmpty())
                {
                    Pager.CurrentPageIndex = pageIndex.ConvertTo<int>();
                }
                else
                {
                    Pager.CurrentPageIndex = 1;
                }
                if (!pageSize.IsNullOrEmpty())
                {
                    Pager.PageSize = pageSize.ConvertTo<int>();
                }
                if (!sortExpression.IsNullOrEmpty())
                {
                    DataSource.SortExpression = sortExpression;
                }
                entities = DataSource.GetList(Pager.CurrentPageIndex, Pager.PageSize, out recordCount);
                Pager.RecordCount = recordCount;
            }
            else
            {
                entities = DataSource.GetList();
            }
            return entities;
        }
    }
}
