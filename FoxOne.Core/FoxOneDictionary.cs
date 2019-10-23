using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public class FoxOneDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : class
    {
        public FoxOneDictionary() :
            base()
        {

        }

        public FoxOneDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {

        }

        public FoxOneDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        { }

        public FoxOneDictionary(int capacity)
            : base(capacity)
        { }

        public FoxOneDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        { }

        public FoxOneDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        { }

        public new TValue this[TKey key]
        {
            get
            {
                if (!base.Keys.Contains(key))
                {
                    return string.Empty as TValue;
                }
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
