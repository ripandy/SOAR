using System;
using UnityEngine;

namespace Soar.Variables
{
    /// <summary>
    /// An extended variable systems serializable to/from json string.
    /// Implements IJsonable interface.
    /// </summary>
    /// <typeparam name="T">Type to use on variable system</typeparam>
    public abstract class JsonableVariable<T> : Variable<T>, IJsonable
    {
        /// <summary>
        /// Converts variable to json string.
        /// Formats with pretty print when used in Unity Editor.
        /// Implemented type must be serializable to/from json string.
        /// Primitive type uses JsonableWrapper to serialize, which add "value" property.
        /// Non-primitive type would not have variable's serialized "value" property before implemented type properties.
        /// </summary>
        /// <returns>Converted json string</returns>
        public string ToJsonString()
        {
            var isSimpleType = typeof(T).IsSimpleType();
            return isSimpleType ?
                JsonUtility.ToJson(new JsonableWrapper<T>(Value), Application.isEditor) :
                JsonUtility.ToJson(Value, Application.isEditor);
        }
        
        /// <summary>
        /// Load variable from json string.
        /// Implemented type must be serializable to/from json string.
        /// Primitive type uses JsonableWrapper to serialize, which requires "value" property.
        /// Non-primitive type should not have "value" property before implemented type properties. 
        /// </summary>
        /// <param name="jsonString">json formatted string</param>
        public void FromJsonString(string jsonString)
        {
            var simpleType = Type.IsSimpleType();
            Value = simpleType
                ? JsonUtility.FromJson<JsonableWrapper<T>>(jsonString).value
                : JsonUtility.FromJson<T>(jsonString);
        }
    }

    public interface IJsonable
    {
        string ToJsonString();
        void FromJsonString(string jsonString);
    }
    
    [Serializable]
    public struct JsonableWrapper<T>
    {
        public T value;
        public JsonableWrapper(T value)
        {
            this.value = value;
        }
    }
}