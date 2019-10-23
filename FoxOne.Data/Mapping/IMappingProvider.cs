
using System.Collections.Generic;
namespace FoxOne.Data.Mapping
{
    public interface IMappingProvider
    {
        bool Supports(string dbProviderName);

        IList<Table> ReadTables(Dao dao);

        string CreateTableCommand(Table mapping);

        string GetDropTableCommand(Table table);

        string CreateSelectStatement(TableMapping mapping);

        string CreateInsertStatement(TableMapping mapping);

        string CreateUpdateStatement(TableMapping mapping);

        string CreateDeleteStatement(TableMapping mapping);

        string CreateGetOneStatement(TableMapping mapping);

        ISqlCommand CreateSelectCommand(TableMapping mapping, object parameters);

        ISqlCommand CreateSelectCountCommand(TableMapping mapping, object parameters);

        ISqlCommand CreateSelectAllCommand(TableMapping mapping, object parameter);

        ISqlCommand CreateInsertCommand(TableMapping mapping, object parameters);

        ISqlCommand CreateUpdateCommand(TableMapping mapping, object parameters);

        ISqlCommand CreateUpdateCommand(TableMapping mapping, object parameters, string[] fields, bool inclusive);

        ISqlCommand CreateDeleteCommand(TableMapping mapping, object parameters);


        ISqlCommand CreateBatchUpdateCommand(TableMapping mapping, object parameters, object whereParameter);

        ISqlCommand CreateBatchDeleteCommand(TableMapping mapping, object whereParameter);
    }
}