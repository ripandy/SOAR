using NUnit.Framework;
using Soar.Collections;
using Soar.Variables;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Soar.Jsonable.Tests
{
    [Serializable]
    public struct JsonableTestStruct
    {
        public int intValue;
        public string stringValue;
        public bool boolValue;
    }

    public class TestJsonableVariable : JsonableVariable<JsonableTestStruct> { }
    public class TestJsonableList : JsonableList<int> { }
    public class TestJsonableDictionary : JsonableDictionary<string, int> { }

    public class JsonableTests
    {
        private TestJsonableVariable variable;
        private TestJsonableList list;
        private TestJsonableDictionary dictionary;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            variable = ScriptableObject.CreateInstance<TestJsonableVariable>();
            list = ScriptableObject.CreateInstance<TestJsonableList>();
            dictionary = ScriptableObject.CreateInstance<TestJsonableDictionary>();
        }

        [Test]
        public void JsonableVariable_ToJsonString_IsValidJson()
        {
            var data = new JsonableTestStruct
            {
                intValue = 42,
                stringValue = "Test",
                boolValue = true
            };
            variable.Value = data;

            var json = variable.ToJsonString();

            Assert.IsNotNull(json);
            Assert.IsTrue(json.StartsWith("{"), "JSON should start with {");
            Assert.IsTrue(json.EndsWith("}"), "JSON should end with }");
            Assert.IsTrue(json.Contains("\"intValue\":"), "JSON should contain intValue field");
            Assert.IsTrue(json.Contains("42"), "JSON should contain value 42");
            Assert.IsTrue(json.Contains("\"stringValue\":"), "JSON should contain stringValue field");
            Assert.IsTrue(json.Contains("\"Test\""), "JSON should contain value 'Test'");
            Assert.IsTrue(json.Contains("\"boolValue\":"), "JSON should contain boolValue field");
            Assert.IsTrue(json.Contains("true"), "JSON should contain value true");
        }

        [Test]
        public void JsonableVariable_FromJsonString_PopulatesData()
        {
            const string json = "{\"intValue\":99, \"stringValue\":\"Loaded from JSON\", \"boolValue\":true}";

            variable.FromJsonString(json);

            var data = variable.Value;
            Assert.AreEqual(99, data.intValue, "intValue should be 99");
            Assert.AreEqual("Loaded from JSON", data.stringValue, "stringValue should be 'Loaded from JSON'");
            Assert.AreEqual(true, data.boolValue, "boolValue should be true");
        }

        [Test]
        public void JsonableList_ToJsonString_IsValidJson()
        {
            list.Clear();
            list.Add(10);
            list.Add(20);
            list.Add(30);

            var json = list.ToJsonString();

            Assert.IsNotNull(json);
            Assert.IsTrue(json.StartsWith("{"), "JSON should start with {");
            Assert.IsTrue(json.EndsWith("}"), "JSON should end with }");
            Assert.IsTrue(json.Contains("\"value\":"), "JSON should contain value array");
            Assert.IsTrue(json.Contains("10"), "JSON should contain value of 10");
            Assert.IsTrue(json.Contains("20"), "JSON should contain value of 20");
            Assert.IsTrue(json.Contains("30"), "JSON should contain value of 30");
        }

        [Test]
        public void JsonableList_FromJsonString_PopulatesData()
        {
            const string json = "{\"value\":[1,2,3,4,5]}";

            list.FromJsonString(json);

            Assert.AreEqual(5, list.Count, "List should contain 5 elements");
            Assert.AreEqual(3, list[2], "Third element should be 3");
        }

        [Test]
        public void JsonableDictionary_ToJsonString_IsValidJson()
        {
            dictionary.Clear();
            dictionary.Add("one", 1);
            dictionary.Add("two", 2);

            var json = dictionary.ToJsonString();

            Assert.IsNotNull(json);
            Assert.IsTrue(json.StartsWith("{"), "JSON should start with {");
            Assert.IsTrue(json.EndsWith("}"), "JSON should end with }");
            Assert.IsTrue(json.Contains("\"value\":"), "JSON should contain value array");
            Assert.IsTrue(json.Contains("\"one\""), "JSON should contain string key 'one'");
            Assert.IsTrue(json.Contains("1"), "JSON should contain int value 1");
            Assert.IsTrue(json.Contains("\"two\""), "JSON should contain string key 'two'");
            Assert.IsTrue(json.Contains("2"), "JSON should contain int value 2");
        }

        [Test]
        public void JsonableDictionary_FromJsonString_PopulatesData()
        {
            // MEMO: SerializedKeyValuePair expects backing fields for serialization.
            const string json = "{\"value\":[{\"<Key>k__BackingField\":\"ten\",\"<Value>k__BackingField\":10},{\"<Key>k__BackingField\":\"twenty\",\"<Value>k__BackingField\":20}]}";
            
            dictionary.FromJsonString(json);

            Assert.AreEqual(2, dictionary.Count, "Dictionary should contain 2 elements");
            Assert.AreEqual(20, dictionary["twenty"], "Value for key 'twenty' should be 20");
        }
        
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Object.DestroyImmediate(variable);
            Object.DestroyImmediate(list);
            Object.DestroyImmediate(dictionary);
        }
    }
}
