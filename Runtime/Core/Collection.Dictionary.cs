using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Soar.Collections
{
    public abstract partial class Dictionary<TKey, TValue> :
        Collection<SerializedKeyValuePair<TKey, TValue>>,
        IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
    {
        private readonly System.Collections.Generic.Dictionary<TKey, TValue> dictionary = new();

        public override object SyncRoot => syncRoot;
        private readonly object syncRoot = new();
        
        public TValue this[TKey key]
        {
            get
            {
                lock (syncRoot)
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        OnValidate();
                    }
                    return dictionary[key];   
                }
            }
            set
            {
                lock (syncRoot)
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        OnValidate();
                    }

                    dictionary[key] = value;
                    
                    var index = list.FindIndex(p => p.Key.Equals(key));
                    var pair = list[index];
                    pair.Value = value;
                    list[index] = pair;

                    RaiseValue(key, value);
                }
            }
        }
        
        public ICollection<TKey> Keys
        {
            get
            {
                lock (syncRoot)
                {
                    return dictionary.Keys;
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                lock (syncRoot)
                {
                    return dictionary.Values;
                }
            }
        }
        
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                lock (syncRoot)
                {
                    return dictionary.Keys;
                }
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                lock (syncRoot)
                {
                    return dictionary.Values;
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                dictionary.Add(item.Key, item.Value);
                base.AddInternal(item);
                RaiseValue(item.Key, item.Value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            AddInternal(new SerializedKeyValuePair<TKey, TValue>(key, value));
        }
        
        internal override void AddInternal(SerializedKeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                dictionary.Add(item.Key, item.Value);
                base.AddInternal(item);
                RaiseValue(item.Key, item.Value);
            }
        }

        internal override void AddRangeInternal(SerializedKeyValuePair<TKey, TValue>[] items)
        {
            lock (syncRoot)
            {
                base.AddRangeInternal(items);
                foreach (var item in items)
                {
                    dictionary.Add(item.Key, item.Value);
                    RaiseValue(item.Key, item.Value);
                }
            }
        }

        internal override void ClearInternal()
        {
            lock (syncRoot)
            {
                base.ClearInternal();
                OnValidate();
            }
        }
        
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                foreach (var pair in dictionary)
                    if (Equals(pair, item))
                        return true;
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (syncRoot)
            {
                return dictionary.ContainsKey(key);
            }
        }

        internal override void CopyInternal(IEnumerable<SerializedKeyValuePair<TKey, TValue>> others)
        {
            lock (syncRoot)
            {
                base.CopyInternal(others);
                OnValidate();
            }
        }
        
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (syncRoot)
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
                base.CopyTo(array.Select(pair => (SerializedKeyValuePair<TKey, TValue>)pair).ToArray(), arrayIndex);
            }
        }

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (syncRoot)
            {
                foreach (var item in dictionary)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public bool Remove(TKey key)
        {
            lock (syncRoot)
            {
                var removed = dictionary.Remove(key, out var value);
                return removed && base.RemoveInternal(new SerializedKeyValuePair<TKey, TValue>(key, value));
            }
        }

        internal override bool RemoveInternal(SerializedKeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                if (!dictionary.TryGetValue(item.Key, out var value)) return false;
                if (!EqualityComparer<TValue>.Default.Equals(value, item.Value)) return false;
                return Remove(item.Key);
            }
        }
        
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                if (!dictionary.TryGetValue(item.Key, out var value)) return false;
                if (!EqualityComparer<TValue>.Default.Equals(value, item.Value)) return false;
                return Remove(item.Key);
            }
        }
        
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (syncRoot)
            {
                return dictionary.TryGetValue(key, out value);
            }
        }
        
        internal override void Initialize()
        {
            OnValidate();
            base.Initialize();
        }
        
        protected virtual void OnValidate()
        {
            lock (syncRoot)
            {
                dictionary.Clear();
                foreach (var pair in list)
                {
                    dictionary.TryAdd(pair.Key, pair.Value);
                }
            }
        }

        // List of Partial methods. Implemented in each respective integrated Library.
        private partial void RaiseValue(TKey key, TValue value);
        
        // TODO: Summaries
        public partial IDisposable SubscribeOnAdd(Action<TKey, TValue> action);
        public partial IDisposable SubscribeOnRemove(Action<TKey, TValue> action);
        public partial IDisposable SubscribeToValues(Action<TKey, TValue> action);
        public partial IDisposable SubscribeToValues(Action<KeyValuePair<TKey, TValue>> action);
    }
}