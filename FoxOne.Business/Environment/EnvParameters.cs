using FoxOne.Business.Environment;
using FoxOne.Core;
using FoxOne.Data;

namespace FoxOne.Business
{
    public class EnvParameters : ISqlParameters
    {
        public const string Prefix = "Env:";
        private static readonly int PrefixLength = Prefix.Length;
        public object Resolve(string name)
        {
            object value;
            return TryResolve(name, out value) ? value : null;
        }

        public bool TryResolve(string name, out object value)
        {
            if (name.ToUpper().StartsWith(Prefix.ToUpper()))
            {
                string varName = name.Substring(PrefixLength);
                return Env.TryResolve(varName, out value);
            }
            value = null;
            return false;
        }
    }
}