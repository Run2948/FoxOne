using System;
using System.Collections.Generic;
using System.Text;

namespace FoxOne.Data.Provider
{
    internal class OracleProvider : DaoProvider
    {
        private const string ProviderName = "Oracle";
        private const string NameParamFormat = ":{0}";
        private const string DefaultOracleDbProvider = "System.Data.OracleClient";
        private const string OdpNetOracleDbProvider = "Oracle.DataAccess.Client";
        private string _oracleDriverName;

        public OracleProvider()
            : base(ProviderName, NameParamFormat)
        {

        }

        public override bool SupportsDbProvider(string dbProviderName)
        {
            if (DefaultOracleDbProvider.Equals(dbProviderName, StringComparison.OrdinalIgnoreCase))
            {
                _oracleDriverName = DefaultOracleDbProvider;
                return true;
            }
            if (OdpNetOracleDbProvider.Equals(dbProviderName, StringComparison.OrdinalIgnoreCase))
            {
                _oracleDriverName = OdpNetOracleDbProvider;
                return true;
            }
            return false;
        }

        public override string WrapPageSql(string sql, string orderClause, int startRowIndex, int rowCount, out IDictionary<string, object> pageParam)
        {
            sql = RemoveOrderByClause(sql);
            StringBuilder pagingSelect = new StringBuilder(sql.Length + 100);

            pagingSelect.Append("select * from (select * from (select row_.*, rownum rownum_ from ( ");
            pagingSelect.Append(sql);
            if (!string.IsNullOrEmpty(orderClause))
            {
                pagingSelect.Append(" Order By ").Append(orderClause);
            }
            pagingSelect.Append("\n ) row_) where rownum_ <= #").Append(PageParamNameEnd).Append("#) where rownum_ >= #")
                .Append(PageParamNameBegin).Append("#");

            pageParam = new Dictionary<string, object>(2)
                            {
                                {PageParamNameBegin, startRowIndex},
                                {PageParamNameEnd, startRowIndex + rowCount - 1}
                            };

            return pagingSelect.ToString();
        }

        public override string EscapeLikeParamValue(string value)
        {
            return value.Replace("?", @"\?").Replace("_", @"\_").Replace("%", @"\%");
        }

        public override bool NamedParameterMustOneByOne
        {
            get { return OdpNetOracleDbProvider.Equals(_oracleDriverName); }
        }
    }
}