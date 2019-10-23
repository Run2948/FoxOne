using System;
using System.Collections.Generic;
using System.Data;

namespace FoxOne.Data.Util
{
    public static class DataReaderExtenstions
    {
        public static IList<IDictionary<string, object>> ReadDictionaries(this IDataReader reader)
        {
            if (reader.IsClosed)
            {
                throw new InvalidOperationException("reader has been closed");
            }

            List<IDictionary<string, object>> rows = new List<IDictionary<string, object>>();

            IDictionary<string, object> row;

            while ((row = ReadDictionary(reader)) != null)
            {
                rows.Add(row);
            }

            return rows;
        }

        public static IDictionary<string, object> ReadDictionary(this IDataReader reader)
        {
            if (reader.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader.GetValue(i));
                }
                return row;
            }
            return null;
        }
    }
}