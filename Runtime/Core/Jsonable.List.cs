using System.Collections.Generic;
using UnityEngine;

namespace Soar.Collections
{
    /// <summary>
    /// An extended list serializable to/from json string.
    /// Implements IJsonable interface.
    /// </summary>
    /// <typeparam name="T">Type to use on list</typeparam>
    public abstract class JsonableList<T> : SoarList<T>, IJsonable
    {
        /// <summary>
        /// Converts list to json string.
        /// Formats with pretty print when used in Unity Editor.
        /// Implemented type must be serializable to/from json string.
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            // Wrap the list in a serializable object for JsonUtility
            var wrapper = new JsonableWrapper<List<T>>(list);
            return JsonUtility.ToJson(wrapper, Application.isEditor);
        }

        /// <summary>
        /// Load list from json string.
        /// Implemented type must be serializable to/from json string.
        /// </summary>
        /// <param name="json"></param>
        public void FromJsonString(string json)
        {
            if (string.IsNullOrEmpty(json)) return;

            var wrapper = JsonUtility.FromJson<JsonableWrapper<List<T>>>(json);
            
            // Replace the contents. Copy raises OnClear (if not empty), OnAdd for each item, and Count once.
            Copy(wrapper.value);
        }
    }
}
