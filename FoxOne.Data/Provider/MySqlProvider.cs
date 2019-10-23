using FoxOne.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoxOne.Data.Provider
{
    public class MySqlProvider : DaoProvider
    {
        private const string ProviderName = "MySql";
        private const string NameParamFormat = "@{0}";
        private const string MySqlClientDbProvider = "MySql.Data.MySqlClient";

        public MySqlProvider()
            : base(ProviderName, NameParamFormat)
        {

        }

        public override bool SupportsDbProvider(string dbProviderName)
        {
            return MySqlClientDbProvider.Equals(dbProviderName, StringComparison.OrdinalIgnoreCase);
        }

        public override string WrapPageSql(string sql, string orderClause, int startRowIndex, int rowCount, out IDictionary<string, object> pageParam)
        {
            sql = RemoveOrderByClause(sql);
            StringBuilder pagingSelect = new StringBuilder(sql.Length + 100);

            if (!String.IsNullOrEmpty(orderClause))
            {
                orderClause = "order by " + orderClause;
            }
            else
            {
                throw new FoxOneException("Paged query must set orderBy Clause");
            }

            pagingSelect.Append(sql).Append(" ").Append(orderClause).Append(" ").Append("limit ").Append("#").Append(PageParamNameBegin).Append("#").Append(",").Append("#").Append(PageParamNameEnd).Append("#");

            pageParam = new Dictionary<string, object>(2)
                            {
                                {PageParamNameBegin, startRowIndex - 1},
                                {PageParamNameEnd, rowCount}
                            };

            return pagingSelect.ToString();
        }

        public override string EscapeLikeParamValue(string value)
        {
            return value.Replace("_", "/_").Replace("%", "/%").Replace("\\", "\\\\");
        }
    }
}