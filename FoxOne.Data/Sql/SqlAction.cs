using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Data.Sql
{
    public class SqlAction : ISqlAction
    {
        private readonly string _name;
        private readonly string _text;

        public SqlAction(string name,string text)
        {
            _name = name;
            _text = text;
        }

        public string Name { get { return _name; } }

        public string Text { get { return _text; } }
    }
}
