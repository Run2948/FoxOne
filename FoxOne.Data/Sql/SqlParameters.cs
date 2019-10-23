using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FoxOne.Data.Mapping;
using FoxOne.Core;

namespace FoxOne.Data.Sql
{
    public abstract class BaseParameters : ISqlParameters
    {
        public object Resolve(string name)
        {
            object value;
            return TryResolve(name, out value) ? value : null;
        }

        public abstract bool TryResolve(string name, out object value);
    }

    public class SqlParameters : BaseParameters
    {
        private readonly ISqlParameters _params;

        public SqlParameters(object parameters)
        {
            this._params = GetParameters(parameters);
        }

        public override bool TryResolve(string name, out object value)
        {
            bool resolved = false;

            if (null != _params)
            {
                resolved = _params.TryResolve(name, out value);
                value = EmptyStringToNull(value);
            }
            else
            {
                value = null;
            }

            if (!resolved)
            {
                ///若自动匹配的 ISqlParameter不能正常分解SQL参数，于是试图从DaoFactory中获取所有ISqlParameter去分解
                ///若分解成功则把分解出来的值值空
                foreach (ISqlParameters parameter in DaoFactory.Parameters)
                {
                    if (parameter.TryResolve(name, out value))
                    {
                        value = EmptyStringToNull(value);
                        return true;
                    }
                }
            }

            return resolved;
        }

        private static object EmptyStringToNull(object value)
        {
            if (value != null && value is string && (value as string).Length == 0)
            {
                return null;
            }
            return value;
        }

        internal bool IsObjectParameters()
        {
            return _params is ObjectParameters;
        }

        internal static ISqlParameters GetParameters(object parameters)
        {
            if (parameters == null)
            {
                return null;
            }
            else if (parameters is ISqlParameters)
            {
                return (ISqlParameters)parameters;
            }
            else if (parameters is IDictionary<string, object>)
            {
                return new GenericDictionaryParameters((IDictionary<string, object>)parameters);
            }
            else if (parameters is IDictionary)
            {
                return new DictionaryParameters((IDictionary)parameters);
            }
            else
            {
                Type type = parameters.GetType();

                if (!parameters.GetType().IsPrimitive && !parameters.GetType().IsValueType)
                {
                    return new ObjectParameters(parameters);
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format("Not Supported Parameters Type : '{0}'", type.FullName));
                }
            }
        }
    }

    internal sealed class GenericDictionaryParameters : BaseParameters
    {
        private readonly IDictionary<string, object> _params;

        internal GenericDictionaryParameters(IDictionary<string, object> parameters)
        {
            this._params = parameters;
        }

        public override bool TryResolve(string name, out object value)
        {
            return _params.TryGetValue(name, out value);
        }
    }

    internal sealed class DictionaryParameters : BaseParameters
    {
        private readonly IDictionary _params;

        internal DictionaryParameters(IDictionary parameters)
        {
            this._params = parameters;
        }

        public override bool TryResolve(string name, out object value)
        {
            return (value = _params[name]) != null;
        }
    }

    internal sealed class ObjectParameters : BaseParameters
    {
        private readonly Type     _type;
        private readonly FastType _reflection;
        private readonly object   _params;

        internal ObjectParameters(object parameters)
        {
            this._type = parameters.GetType();
            this._reflection = FastType.Get(_type);
            this._params = parameters;
        }

        public override bool TryResolve(string name, out object value)
        {
            FastProperty prop = _reflection.GetGetter(name);

            if (null != prop)
            {
                value = prop.GetValue(_params);
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }

    internal sealed class ArrayParameters : BaseParameters
    {
        private readonly ISqlParameters[] _params;

        internal ArrayParameters(params object[] parameters)
        {
            _params = new ISqlParameters[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                _params[i] = SqlParameters.GetParameters(parameters[i]);
            }
        }

        public override bool TryResolve(string name, out object value)
        {
            foreach (ISqlParameters p in _params)
            {
                if (p.TryResolve(name, out value))
                {
                    return true;
                }
            }
            value = null;
            return false;
        }
    }

    internal sealed class ParametersWrapper : BaseParameters
    {
        private IDictionary<string, object> _items;
        private readonly ISqlParameters _params;

        public ParametersWrapper(ISqlParameters parameters)
        {
            this._params = parameters;
        }

        internal void Add(string name, object value)
        {
            if (null == _items)
            {
                _items = new Dictionary<string, object>();
            }
            _items[name] = value;
        }

        public override bool TryResolve(string name, out object value)
        {
            if (null != _items && _items.TryGetValue(name, out value))
            {
                return true;
            }
            else if (null != _params && _params.TryResolve(name, out value))
            {
                return true;
            }
            value = null;
            return false;
        }
    }
}