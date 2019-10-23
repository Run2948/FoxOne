using System;
using System.Collections.Generic;
using System.Linq;
using FoxOne.Core;
namespace FoxOne.Data.Mapping.Provider
{
    public class SqlServerMappingProvider : GenericMappingProvider
    {
        protected override string NamedParameterFormat
        {
            get { return "@{0}"; }
        }
        public override bool Supports(string dbProviderName)
        {
            return "System.Data.SqlClient".Equals(dbProviderName, StringComparison.OrdinalIgnoreCase);
        }

        protected override string GetTablesSql()
        {
            return @"SELECT TABLE_SCHEMA AS [Schema],TABLE_NAME AS [Name] 
                     FROM INFORMATION_SCHEMA.TABLES 
                     WHERE TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW'";
        }

        protected override string GetColumnsSql()
        {
            return @"SELECT C.TABLE_SCHEMA AS [Schema], 
		                    C.TABLE_NAME AS [Table], 
		                    C.COLUMN_NAME AS [Name],
		                    ISNULL(P.value,C.COLUMN_NAME) [Comment],
		                    CASE WHEN  C.IS_NULLABLE='NO' THEN 0 ELSE 1 END [IsNullable],
		                    C.DATA_TYPE [Type],
		                    C.CHARACTER_MAXIMUM_LENGTH [Length],
		                    C.ORDINAL_POSITION [Rank],
		                    COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') AS [IsAutoIncrement]
                    FROM INFORMATION_SCHEMA.COLUMNS C
                    LEFT JOIN sys.extended_properties P
                    ON  OBJECT_NAME(P.major_id) = C.TABLE_NAME AND P.minor_id = C.ORDINAL_POSITION  AND P.name = 'MS_Description'  
                    ORDER BY TABLE_NAME ASC, ORDINAL_POSITION ASC";
        }

        protected override string GetKeysSql()
        {
            return @"SELECT KCU.TABLE_SCHEMA AS [Schema],
	                        KCU.TABLE_NAME AS [Table],
	                        KCU.COLUMN_NAME AS [Column]
                     FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
                          JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
                          ON (KCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME AND KCU.TABLE_NAME = TC.TABLE_NAME AND KCU.CONSTRAINT_SCHEMA = TC.CONSTRAINT_SCHEMA)
                     WHERE TC.CONSTRAINT_TYPE='PRIMARY KEY'
                     ORDER BY ORDINAL_POSITION ASC";
        }

        protected override string EscapeIdentifier(string name)
        {
            return string.Format("[{0}]", name);
        }

        public override string CreateTableCommand(Table mapping)
        {
            string createTableSQL = "CREATE TABLE [dbo].{0}({1},CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ({2})WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]";
            var keys = new List<string>();
            mapping.Keys.Select(o => o.Name).ForEach(o =>
            {
                keys.Add("[{0}]".FormatTo(o));
            });
            var columns = new List<string>();
            var fields = mapping.Columns;
            foreach (var field in fields)
            {
                columns.Add(GetColumnsSQL(field));
            }
            string keyString = string.Join(",", keys.ToArray());
            return string.Format(createTableSQL, mapping.Name, string.Join(",", columns.ToArray()), string.Format("{0} ASC", keyString));
        }

        private string GetColumnsSQL(Column field)
        {
            return string.Format("[{0}] [{1}]{2} {3} {4}",
                field.Name,
                field.Type,
                string.IsNullOrEmpty(field.Length) ? "" : "(" + field.Length + ")",
                field.IsAutoIncrement ? "IDENTITY(1,1)" : "",
                field.IsNullable ? "NULL" : "NOT NULL"
                );
        }

        /// <summary>
        /// 获取删除数据表的脚本
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override string GetDropTableCommand(Table table)
        {
            return string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'U')) DROP TABLE [{0}]", table.Name);
        }

        protected override string GetSearchCondition(TableMapping mapping)
        {
            var fields = mapping.Table.Columns;
            List<string> condition = new List<string>();
            string[] likeTypes = "char|varchar|nvarchar|text".Split('|');
            string[] dateTypes = "datetime|datetime2".Split('|');
            foreach (var field in fields)
            {
                string temp = string.Empty;
                if (likeTypes.Contains(field.Type))
                {
                    temp = string.Format("\n{{? AND [{0}] like '%${0}$%' }}", field.Name);
                }
                else if (dateTypes.Contains(field.Type))
                {
                    temp = string.Format("\n{{? AND CONVERT(varchar(10),[{0}],120) = #{0}# }}", field.Name);
                }
                else
                {
                    temp = string.Format("\n{{? AND [{0}]=#{0}# }}", field.Name);
                }
                condition.Add(temp);
            }
            return string.Join(" ", condition.ToArray());
        }
    }
}