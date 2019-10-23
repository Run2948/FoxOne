using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace FoxOne.Core
{
    public class FastType
    {
        private static readonly Dictionary<Type, FastType> _cache = new Dictionary<Type, FastType>();
        private static readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

        public static FastType Get(Type type)
        {
            return Get(type,null);
        }

        protected static FastType Get(Type type,Func<Type,FastType> CreateFastType)
        {
            FastType value;

            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (!_cache.TryGetValue(type, out value))
                {
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        value = CreateFastType == null ? new FastType(type) : CreateFastType(type);
                        _cache[type] = value;
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
            return value;  
        }

        protected Type _type;
        protected FastProperty[] _setters;
        protected FastProperty[] _getters;

        public FastType(Type type)
        {
            this._type = type;
            this.Initialize();
        }

        public FastProperty GetGetter(string FastPropertyName)
        {
            return _getters.SingleOrDefault(p => p.Info.Name.Equals(FastPropertyName,StringComparison.OrdinalIgnoreCase));
        }

        public FastProperty GetSetter(string FastPropertyName)
        {
            return _setters.SingleOrDefault(p => p.Info.Name.Equals(FastPropertyName, StringComparison.OrdinalIgnoreCase));
        }

        public FastProperty[] Getters
        {
            get { return _getters; }
        }

        public FastProperty[] Setters
        {
            get { return _setters; }
        }

        private void Initialize()
        {
            this.InitializeProperties();
            this.InitializeMethods();
        }

        protected virtual void InitializeProperties()
        {
            List<FastProperty> setters = new List<FastProperty>();
            List<FastProperty> getters = new List<FastProperty>();

            foreach (PropertyInfo prop in GetProperties(_type))
            {
                String columnName = GetPropertyName(prop);
                FastProperty mapping = new FastProperty(columnName, prop);

                if (prop.CanRead)
                {
                    getters.Add(mapping);
                }

                if (prop.CanWrite)
                {
                    setters.Add(mapping);
                }
            }
            _setters = setters.ToArray();
            _getters = getters.ToArray();
        }

        protected virtual void InitializeMethods()
        {
            //do nothing
        }

        protected virtual IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        protected virtual String GetPropertyName(PropertyInfo p)
        {
            return p.Name;
        }
    }
}
