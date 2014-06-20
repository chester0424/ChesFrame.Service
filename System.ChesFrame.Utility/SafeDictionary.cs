using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility
{
    public class SafeDictionary<TKey, TValue>
    {
        readonly object _syncLock = new object();
        readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            set { lock (_syncLock) _dictionary[key] = value; }
            get { lock (_syncLock) return _dictionary[key]; }
        }

        public IEnumerable<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_syncLock) return _dictionary.TryGetValue(key, out value);
        }

        public bool Remove(TKey key)
        {
            lock (_syncLock) return _dictionary.Remove(key);
        }

        public void Clear()
        {
            lock (_syncLock) _dictionary.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            lock (_syncLock) return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            lock (_syncLock) _dictionary.Add(key, value);
        }
    }
}
