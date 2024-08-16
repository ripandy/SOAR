using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soar.Collections
{
    public abstract partial class Collection<TKey, TValue> :
        Collection<SerializedKeyValuePair<TKey, TValue>>,
        IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> dictionary = new();
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
        
        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                lock (syncRoot)
                {
                    return dictionary.Keys;
                }
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
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

        public override void Add(SerializedKeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                dictionary.Add(item.Key, item.Value);
                base.Add(item);
                RaiseValue(item.Key, item.Value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                dictionary.Add(item.Key, item.Value);
                base.Add(item);
                RaiseValue(item.Key, item.Value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            Add(new SerializedKeyValuePair<TKey, TValue>(key, value));
        }

        public override void Clear()
        {
            lock (syncRoot)
            {
                ClearValueSubscriptions();
                base.Clear();
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

        public override void Copy(IEnumerable<SerializedKeyValuePair<TKey, TValue>> others)
        {
            lock (syncRoot)
            {
                base.Copy(others);
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

        public bool Remove(TKey key)
        {
            lock (syncRoot)
            {
                var removed = dictionary.Remove(key, out var value);
                RemoveValueSubscription(key);
                return removed && base.Remove(new SerializedKeyValuePair<TKey, TValue>(key, value));
            }
        }

        public override bool Remove(SerializedKeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                if (!dictionary.TryGetValue(item.Key, out var value)) return false;
                if (!EqualityComparer<TValue>.Default.Equals(value, item.Value)) return false;
                
                if (dictionary.Remove(item.Key))
                {
                    RemoveValueSubscription(item.Key);
                }
                
                return base.Remove(item);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                if (!dictionary.TryGetValue(item.Key, out var value)) return false;
                if (!EqualityComparer<TValue>.Default.Equals(value, item.Value)) return false;
                
                if (dictionary.Remove(item.Key))
                {
                    RemoveValueSubscription(item.Key);
                }
                
                return base.Remove(item);
            }
        }
        
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (syncRoot)
            {
                return dictionary.TryGetValue(key, out value);
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

        private void OnValidate()
        {
            dictionary.Clear();
            foreach (var pair in list)
            {
                dictionary.TryAdd(pair.Key, pair.Value);
            }
        }

        // List of Partial methods. Implemented in each respective integrated Library.
        public partial IDisposable SubscribeToValue(TKey key, Action<TValue> action);
        
        private partial void RaiseValue(TKey key, TValue value);
        private partial void ClearValueSubscriptions();
        private partial void RemoveValueSubscription(TKey key);
    }

    [Serializable]
    public struct SerializedKeyValuePair<TKey, TValue>
    {
        [field:SerializeField] public TKey Key { get; internal set; }
        [field:SerializeField] public TValue Value { get; internal set; }
    
        public SerializedKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
        
        public static implicit operator KeyValuePair<TKey, TValue>(SerializedKeyValuePair<TKey, TValue> serializedKeyValuePair) => new(serializedKeyValuePair.Key, serializedKeyValuePair.Value);
        public static implicit operator SerializedKeyValuePair<TKey, TValue>(KeyValuePair<TKey, TValue> keyValuePair) => new(keyValuePair.Key, keyValuePair.Value);
    }
}