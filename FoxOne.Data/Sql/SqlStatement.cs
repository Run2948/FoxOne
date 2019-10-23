using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Data.Provider;

namespace FoxOne.Data.Sql
{
    public class SqlStatement : ISqlStatement
    {
        private readonly string           _text;
        private readonly IList<SqlClause> _clauses;
        private bool? _isQuery;

        public SqlStatement(string text,IList<SqlClause> clauses)
        {
            this._text = text;
            this._clauses = clauses;
        }

        public bool IsQuery
        {
            get
            {
                if (_isQuery == null)
                {
                    if (!string.IsNullOrEmpty(Text))
                    {
                        _isQuery = Text.Trim().ToLower().StartsWith("select ");
                        return _isQuery.Value;
                    }
                    else
                    {
                        return false;
                    }
                }
                return _isQuery.Value;
            }
        }

        public string Text
        {
            get { return _text; }
        }

        public string Connection { get; set; }

        public IList<SqlClause> Clauses
        {
            get { return _clauses; }
        }

        public ISqlCommand CreateCommand(IDaoProvider provider, object parameters)
        {
            SqlCommandBuilder builder   = new SqlCommandBuilder();
            SqlParameters sqlParameters = new SqlParameters(parameters);

            foreach (SqlClause clause in _clauses)
            {
                clause.ToCommand(provider, builder, sqlParameters);
            }

            return builder.ToCommand();
        }

    }
}