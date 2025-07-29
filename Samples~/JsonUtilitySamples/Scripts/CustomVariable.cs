using Soar.Variables;
using System;
using UnityEngine;

namespace Soar.Json.Sample
{
    [CreateAssetMenu(fileName = "CustomVariable", menuName = MenuHelper.DefaultVariableMenu + "Custom")]
    public class CustomVariable : JsonableVariable<CustomStruct>
    {
    }

    [Serializable]
    public struct CustomStruct
    {
        public bool boolField;
        public int intField;
        public float floatField;
        public string stringField;
    }
}