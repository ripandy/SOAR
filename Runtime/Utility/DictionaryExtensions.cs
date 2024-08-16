using System.Collections.Generic;

namespace Soar
{
    public static class DictionaryExtensions
    {
        // source : https://stackoverflow.com/a/15728577
        public static bool TryChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey oldKey, TKey newKey)
        {
            if (!dict.Remove(oldKey, out var value)) return false;

            dict[newKey] = value;  // or dict.Add(newKey, value) depending on ur comfort
            return true;
        }
    }
}