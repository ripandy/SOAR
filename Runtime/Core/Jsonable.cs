using System;

namespace Soar
{
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