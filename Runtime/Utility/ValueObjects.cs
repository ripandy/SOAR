using System;
using System.Collections.Generic;
using UnityEngine;

namespace Soar
{
    public readonly struct PairwiseValue<T>
    {
        public T OldValue { get; }
        public T NewValue { get; }
        
        public PairwiseValue(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
    
    public readonly struct IndexValuePair<T>
    {
        public int Index { get; }
        public T Value { get; }
        
        public IndexValuePair(int index, T value)
        {
            Index = index;
            Value = value;
        }
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