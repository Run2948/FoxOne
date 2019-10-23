using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
namespace FoxOne.Data.Mapping.Provider
{
    public class SqlServerCeMappingProvider : GenericMappingProvider
    {
        protected override string NamedParameterFormat
        {
            get { return "@{0}"; }
        }

        public override bool Supports(string dbProviderName)
        {
            return dbProviderName.StartsWith("System.Data.SqlServerCe", StringComparison.OrdinalIgnoreCase);
        }

        protected override string GetTablesSql()
        {
            return @"SELECT TABLE_SCHEMA AS [Schema],
                            TABLE_NAME AS [Name] 
                            FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_TYPE='TABLE'";
        }

        protected override string GetColumnsSql()
        {
            return @"SELECT TABLE_SCHEMA AS [Schema], 
			                TABLE_NAME AS [Table], 
			                COLUMN_NAME AS [Name],
                            AUTOINC_INCREMENT AS [IsAutoIncrement]
		             FROM INFORMATION_SCHEMA.COLUMNS
		             ORDER BY ORDINAL_POSITION ASC";
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
            return string.Format("[{0}]",name);
        }

        public override string CreateTableCommand(Table mapping)
        {
            throw new NotImplementedException();
        }

        public override string GetDropTableCommand(Table table)
        {
            throw new NotImplementedException();
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