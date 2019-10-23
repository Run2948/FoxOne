using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using FoxOne.Core;
namespace FoxOne.Business.Environment
{
    public class DefaulttProvider : IEnvironmentProvider
    {
        public string Prefix
        {
            get
            {
                return "Default";
            }
        }

        public object Resolve(string name)
        {
            if(name.Equals("Guid", StringComparison.OrdinalIgnoreCase))
            {
                return Guid.NewGuid();
            }
            else if(name.Equals("Now", StringComparison.OrdinalIgnoreCase))
            {
                return DateTime.Now;
            }
            return null;
        }
    }
}