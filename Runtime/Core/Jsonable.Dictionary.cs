using System.Collections.Generic;
using UnityEngine;

namespace Soar.Collections
{
    /// <summary>
    /// An extended dictionary serializable to/from json string.
    /// Implements IJsonable interface.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class JsonableDictionary<TKey, TValue> : SoarDictionary<TKey, TValue>, IJsonable where TKey : notnull
    {
        /// <summary>
        /// Converts dictionary to json string.
        /// Formats with pretty print when used in Unity Editor.
        /// Implemented type must be serializable to/from json string.
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            // Wrap the underlying list of key-value pairs for serialization.
            var wrapper = new JsonableWrapper<List<SerializedKeyValuePair<TKey, TValue>>>(list);
            return JsonUtility.ToJson(wrapper, Application.isEditor);
        }

        /// <summary>
        /// Load dictionary from json string.
        /// Implemented type must be serializable to/from json string.
        /// </summary>
        /// <param name="json"></param>
        public void FromJsonString(string json)
        {
            if (string.IsNullOrEmpty(json)) return;

            var wrapper = JsonUtility.FromJson<JsonableWrapper<List<SerializedKeyValuePair<TKey, TValue>>>>(json);
            
            // Clear the current dictionary and list.
            Clear();
            
            // Add the loaded items. The internal methods will handle rebuilding the dictionary lookup.
            AddRangeInternal(wrapper.value.ToArray());
        }
    }
}
