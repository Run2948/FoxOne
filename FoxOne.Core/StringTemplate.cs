using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FoxOne.Core
{
    public class StringTemplate
    {
        public static readonly Regex Pattern = new Regex(@"\$(?<Variable>.+?)\$", RegexOptions.Compiled);
        public string ItemTemplate { get; set; }

        private IDictionary<string,object> Data;
        public StringTemplate(string itemTemplate)
        {
            ItemTemplate = itemTemplate;
        }

        public void SetAttribute(object value)
        {
            Data = value.ToDictionary();
        }

        private object Resolve(string name)
        {
            if (Data.IsNullOrEmpty() || !Data.Keys.Contains(name,StringComparer.OrdinalIgnoreCase)) return string.Empty;
            return Data[name];
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            var arr = Pattern.Split(ItemTemplate);
            for (int i = 0; i < arr.Length; i++)
            {
                builder.Append(arr[i++]);
                if (i < arr.Length)
                {
                    builder.Append(Resolve(arr[i]));
                }
            }
            return builder.ToString();
        }
    }
}
