using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace FoxOne.Core
{
    [Serializable]
    public class SessionState:IDisposable
    {
        private IDictionary<string, object> _items   = new Dictionary<string, object>();
        
        public object this[string name]
        {
            get
            {
                lock (this)
                {
                    object value;
                    return _items.TryGetValue(name, out value) ? value : null;
                }
            }
            set
            {
                lock (this)
                {
                    _items[name] = value;
                }
            }
        }

        public object GetValue(string name, System.Func<object> value)
        {
            lock (this)
            {
                object returnValue;
                if (!_items.TryGetValue(name, out returnValue))
                {
                    returnValue = value();
                    _items[name] = returnValue;
                }
                return returnValue;
            }
        }

        public bool TryGetValue(string name, out object value)
        {
            lock (this)
            {
                return _items.TryGetValue(name, out value);
            }
        }

        public bool Remove(string name)
        {
            lock (this)
            {
                return _items.Remove(name);
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                _items.Clear();
            }
        }
    }
}