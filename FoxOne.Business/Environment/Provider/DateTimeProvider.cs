using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxOne.Core;
namespace FoxOne.Business.Environment
{
    public class DateTimeProvider : IEnvironmentProvider
    {

        public string Prefix
        {
            get
            {
                return "DateTime";
            }
        }

        public object Resolve(string name)
        {
            var days = name.ConvertTo<int>();
            var dt = DateTime.Now.AddDays(days);
            return dt;
        }
    }
}
