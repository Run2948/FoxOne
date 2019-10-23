using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FoxOne.Core;

namespace FoxOne.Business.Environment
{
    public static class Env
    {
        private static EnvironmentContainer _container;
        static Env()
        {
            _container = new EnvironmentContainer();
        }
        public static string Parse(string expression)
        {
            return _container.Parse(expression);
        }

        public static object Resolve(string name)
        {
            object value;
            TryResolve(name, out value);
            return value;
        }

        public static bool TryResolve(string name, out object value)
        {
            return _container.TryResolve(name, out value);
        }
    }
}