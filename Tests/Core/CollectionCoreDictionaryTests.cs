using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Soar.Collections.Tests
{
    public partial class CollectionCoreDictionaryTests
    {
        private class NumberStringCollection : Collection<NumberEnum, string> { }
        private NumberStringCollection testNumberStringCollection;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            testNumberStringCollection = ScriptableObject.CreateInstance<NumberStringCollection>();
        }

        [Test]
        public void SubscribeOnAdd_ShouldBeListened()
        {
            testNumberStringCollection.Clear();
            
            var addedValues = new List<KeyValuePair<NumberEnum, string>>();
            var subscription = testNumberStringCollection.SubscribeOnAdd(addedValue => addedValues.Add(addedValue));

            testNumberStringCollection.Add(NumberEnum.One, NumberEnum.One.ToString());
            Assert.AreEqual(NumberEnum.One, addedValues[0].Key, "Simple add: key.");
            Assert.AreEqual(NumberEnum.One.ToString(), addedValues[0].Value, "Simple add: value.");
            
            testNumberStringCollection.Add(new KeyValuePair<NumberEnum, string>(NumberEnum.Two, NumberEnum.Two.ToString()));
            Assert.AreEqual(NumberEnum.Two, addedValues[1].Key, "Add KeyValuePair: key.");
            Assert.AreEqual(NumberEnum.Two.ToString(), addedValues[1].Value, "Add KeyValuePair: value.");
            
            testNumberStringCollection.Add(new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Three, NumberEnum.Three.ToString()));
            Assert.AreEqual(NumberEnum.Three, addedValues[2].Key, "Add SerializedKeyValuePair: key.");
            Assert.AreEqual(NumberEnum.Three.ToString(), addedValues[2].Value, "Add SerializedKeyValuePair: value.");
            
            var otherAddedValues = new List<(NumberEnum Key, string Value)>();
            using var otherSubscription = testNumberStringCollection.SubscribeOnAdd((key, value) => otherAddedValues.Add((key, value)));
            
            testNumberStringCollection.Add(NumberEnum.Four, NumberEnum.Four.ToString());
            testNumberStringCollection.Add(NumberEnum.Five, NumberEnum.Five.ToString());
            testNumberStringCollection.Add(NumberEnum.Six, NumberEnum.Six.ToString());
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Four, NumberEnum.Four.ToString()), addedValues[3], "Added multiple times.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Five, NumberEnum.Five.ToString()), addedValues[4], "Added multiple times.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Six, NumberEnum.Six.ToString()), addedValues[5], "Added multiple times.");
            Assert.AreEqual((Key: NumberEnum.Four, Value: NumberEnum.Four.ToString()), otherAddedValues[0], "Added multiple times.");
            Assert.AreEqual((Key: NumberEnum.Five, Value: NumberEnum.Five.ToString()), otherAddedValues[1], "Added multiple times.");
            Assert.AreEqual((Key: NumberEnum.Six, Value: NumberEnum.Six.ToString()), otherAddedValues[2], "Added multiple times.");
            
            Assert.AreEqual(6, addedValues.Count, "Added 6 elements before subscription dispose.");
            subscription.Dispose();
            
            testNumberStringCollection.Add(NumberEnum.Seven, NumberEnum.Seven.ToString());
            Assert.AreEqual(6, addedValues.Count, "Added value count should not be updated due to subscription has been disposed");
            Assert.AreEqual(NumberEnum.Seven, otherAddedValues[3].Key, "Simple add: key.");
            Assert.AreEqual(NumberEnum.Seven.ToString(), otherAddedValues[3].Value, "Simple add: value.");
            
            AssertCollectionContents();
        }
        
        [Test]
        public void SubscribeOnAdd_ByBaseClassAddRange_ShouldBeListened()
        {
            testNumberStringCollection.Clear();
            
            var addedValues = new List<KeyValuePair<NumberEnum, string>>();
            using var subscription = testNumberStringCollection.SubscribeOnAdd(addedValue => addedValues.Add(addedValue));

            testNumberStringCollection.AddRange(new []
            {
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.One, NumberEnum.One.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Three, NumberEnum.Three.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Five, NumberEnum.Five.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Seven, NumberEnum.Seven.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Nine, NumberEnum.Nine.ToString())
            });
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.One, NumberEnum.One.ToString()), addedValues[0], "Add Range.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Three, NumberEnum.Three.ToString()), addedValues[1], "Add Range.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Five, NumberEnum.Five.ToString()), addedValues[2], "Add Range.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Seven, NumberEnum.Seven.ToString()), addedValues[3], "Add Range.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Nine, NumberEnum.Nine.ToString()), addedValues[4], "Add Range.");
            
            testNumberStringCollection.AddRange(Enumerable.Range(1, 5).Select(ConvertFromIndex));
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Two, NumberEnum.Two.ToString()), addedValues[5], "Add Range Enumerable.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Four, NumberEnum.Four.ToString()), addedValues[6], "Add Range Enumerable.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Six, NumberEnum.Six.ToString()), addedValues[7], "Add Range Enumerable.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Eight, NumberEnum.Eight.ToString()), addedValues[8], "Add Range Enumerable.");
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Ten, NumberEnum.Ten.ToString()), addedValues[9], "Add Range Enumerable.");

            AssertCollectionContents();
            
            SerializedKeyValuePair<NumberEnum, string> ConvertFromIndex(int val)
            {
                var key = (NumberEnum)(val * 2);
                var value = key.ToString();
                return new SerializedKeyValuePair<NumberEnum, string>(key, value);
            }
        }
        
        [Test]
        public void SubscribeOnRemove_ShouldBeListened()
        {
            testNumberStringCollection.Clear();
            testNumberStringCollection.Add(NumberEnum.One, NumberEnum.One.ToString());
            testNumberStringCollection.Add(NumberEnum.Two, NumberEnum.Two.ToString());
            testNumberStringCollection.Add(NumberEnum.Three, NumberEnum.Three.ToString());
            testNumberStringCollection.Add(NumberEnum.Four, NumberEnum.Four.ToString());
            testNumberStringCollection.Add(NumberEnum.Ten, NumberEnum.Ten.ToString());
            
            var removedValue = new KeyValuePair<NumberEnum, string>();
            var subscription = testNumberStringCollection.SubscribeOnRemove(removedVal => removedValue = removedVal);

            var removed = testNumberStringCollection.Remove(NumberEnum.One);
            Assert.AreEqual(NumberEnum.One, removedValue.Key, "Simple remove: key.");
            Assert.AreEqual(NumberEnum.One.ToString(), removedValue.Value, "Simple remove: value.");
            Assert.IsTrue(removed, "Removed is true.");
            
            removed = testNumberStringCollection.Remove(NumberEnum.Five);
            Assert.AreEqual(NumberEnum.One, removedValue.Key, "Should not be updated due to key does not exist.");
            Assert.AreEqual(NumberEnum.One.ToString(), removedValue.Value, "Should not be updated due to key does not exist.");
            Assert.IsFalse(removed, "Removed is false.");
            
            removed = testNumberStringCollection.Remove(new KeyValuePair<NumberEnum, string>(NumberEnum.Two, NumberEnum.Two.ToString()));
            Assert.AreEqual(NumberEnum.Two, removedValue.Key, "Remove KeyValuePair: key.");
            Assert.AreEqual(NumberEnum.Two.ToString(), removedValue.Value, "Remove KeyValuePair: value.");
            Assert.IsTrue(removed, "Removed is true.");
            
            removed = testNumberStringCollection.Remove(new KeyValuePair<NumberEnum, string>(NumberEnum.Six, NumberEnum.Six.ToString()));
            Assert.AreEqual(NumberEnum.Two, removedValue.Key, "Should not be updated due to key does not exist.");
            Assert.AreEqual(NumberEnum.Two.ToString(), removedValue.Value, "Should not be updated due to key does not exist.");
            Assert.IsFalse(removed, "Removed is false.");
            
            (NumberEnum Key, string Value) otherRemovedValue = (NumberEnum.Zero, string.Empty);
            using var otherSubscription = testNumberStringCollection.SubscribeOnRemove((key, value) => otherRemovedValue = (key, value));
            
            removed = testNumberStringCollection.Remove(new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Ten, NumberEnum.Ten.ToString()));
            Assert.AreEqual(NumberEnum.Ten, removedValue.Key, "Remove SerializedKeyValuePair: key.");
            Assert.AreEqual(NumberEnum.Ten.ToString(), removedValue.Value, "Remove SerializedKeyValuePair: value.");
            Assert.AreEqual((Key: NumberEnum.Ten, Value: NumberEnum.Ten.ToString()), otherRemovedValue, "Remove SerializedKeyValuePair: key and value.");
            Assert.IsTrue(removed, "Removed is true.");
            
            removed = testNumberStringCollection.Remove(new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Nine, NumberEnum.Nine.ToString()));
            Assert.AreEqual(NumberEnum.Ten, removedValue.Key, "Should not be updated due to key does not exist.");
            Assert.AreEqual(NumberEnum.Ten.ToString(), removedValue.Value, "Should not be updated due to key does not exist.");
            Assert.AreEqual((Key: NumberEnum.Ten, Value: NumberEnum.Ten.ToString()), otherRemovedValue, "Should not be updated due to key does not exist.");
            Assert.IsFalse(removed, "Removed is false.");
            
            subscription.Dispose();
            
            removed = testNumberStringCollection.Remove(NumberEnum.Four);
            Assert.AreEqual(NumberEnum.Ten, removedValue.Key, "Should not be updated due to subscription has been disposed");
            Assert.AreEqual(NumberEnum.Ten.ToString(), removedValue.Value, "Should not be updated due to subscription has been disposed");
            Assert.IsTrue(removed, "Even if subscription is disposed, removal should be successful.");
            
            AssertCollectionContents();
        }
        
        [Test]
        public void SubscribeOnClear_ShouldBeListened()
        {
            testNumberStringCollection.Clear();
            
            var cleared = false;
            var subscription = testNumberStringCollection.SubscribeOnClear(() => cleared = true);
            
            testNumberStringCollection.Add(NumberEnum.One, NumberEnum.One.ToString());
            testNumberStringCollection.Add(NumberEnum.Two, NumberEnum.Two.ToString());
            testNumberStringCollection.Add(NumberEnum.Three, NumberEnum.Three.ToString());
            testNumberStringCollection.Add(NumberEnum.Four, NumberEnum.Four.ToString());
            testNumberStringCollection.Add(NumberEnum.Ten, NumberEnum.Ten.ToString());
            testNumberStringCollection.Clear();
            
            Assert.IsTrue(cleared, "Clear collection.");

            cleared = false;
            subscription.Dispose();
            
            testNumberStringCollection.Add(NumberEnum.Five, NumberEnum.Five.ToString());
            testNumberStringCollection.Add(NumberEnum.Six, NumberEnum.Six.ToString());
            testNumberStringCollection.Add(NumberEnum.Seven, NumberEnum.Seven.ToString());
            testNumberStringCollection.Clear();
            
            Assert.IsFalse(cleared, "Should not be updated due to subscription has been disposed");
            
            AssertCollectionContents();
        }
        
        [Test]
        public void SubscribeToCount_ShouldBeListened()
        {
            testNumberStringCollection.Clear();
            
            var countValue = 0;
            var subscription = testNumberStringCollection.SubscribeToCount(count => countValue = count);
            
            testNumberStringCollection.Add(NumberEnum.One, NumberEnum.One.ToString());
            testNumberStringCollection.Add(NumberEnum.Two, NumberEnum.Two.ToString());
            testNumberStringCollection.Add(NumberEnum.Three, NumberEnum.Three.ToString());
            testNumberStringCollection.Add(NumberEnum.Four, NumberEnum.Four.ToString());
            testNumberStringCollection.Add(NumberEnum.Ten, NumberEnum.Ten.ToString());
            Assert.AreEqual(5, countValue, "Added 5 elements.");
            
            testNumberStringCollection.Remove(NumberEnum.Two);
            Assert.AreEqual(4, countValue, "Removed 1 time from count = 4.");
            
            testNumberStringCollection.Clear();
            Assert.AreEqual(0, countValue, "Cleared collection.");
            
            subscription.Dispose();
            
            testNumberStringCollection.Add(NumberEnum.Five, NumberEnum.Five.ToString());
            testNumberStringCollection.Add(NumberEnum.Six, NumberEnum.Six.ToString());
            testNumberStringCollection.Add(NumberEnum.Seven, NumberEnum.Seven.ToString());
            Assert.AreEqual(0, countValue, "Should not be updated due to subscription has been disposed");
            
            AssertCollectionContents();
        }
        
        [Test]
        public void SubscribeToValues_ShouldBeListened()
        {
            testNumberStringCollection.Clear();
            testNumberStringCollection.Add(NumberEnum.One, NumberEnum.One.ToString());
            testNumberStringCollection.Add(NumberEnum.Two, NumberEnum.Two.ToString());
            testNumberStringCollection.Add(NumberEnum.Three, NumberEnum.Three.ToString());
            testNumberStringCollection.Add(NumberEnum.Four, NumberEnum.Four.ToString());
            testNumberStringCollection.Add(NumberEnum.Ten, NumberEnum.Ten.ToString());
            
            var element = new KeyValuePair<NumberEnum, string>();
            var subscription = testNumberStringCollection.SubscribeToValues(pair => element = pair);
            Assert.AreEqual(NumberEnum.Zero, element.Key, "Should be default index since subscription happened late.");
            Assert.AreEqual(null, element.Value, "Should be default value since subscription happened late.");

            testNumberStringCollection[NumberEnum.One] = "Satu";
            Assert.AreEqual(NumberEnum.One, element.Key, "Element with key One changed.");
            Assert.AreEqual("Satu", element.Value, "Element is changed.");
            
            testNumberStringCollection.Add(NumberEnum.Five, NumberEnum.Five.ToString());
            
            var otherElement = (Key: NumberEnum.Zero, Value: string.Empty);
            using var subscription2 = testNumberStringCollection.SubscribeToValues((key, value) => otherElement = (key, value));
            
            testNumberStringCollection.Add(NumberEnum.Six, NumberEnum.Six.ToString());
            testNumberStringCollection.Add(NumberEnum.Seven, NumberEnum.Seven.ToString());
            Assert.AreEqual(NumberEnum.Seven, element.Key, "Element is last added pair: key.");
            Assert.AreEqual(NumberEnum.Seven.ToString(), element.Value, "Element is last added pair: value");
            Assert.AreEqual(NumberEnum.Seven, otherElement.Key, "Element is last added pair: key.");
            Assert.AreEqual(NumberEnum.Seven.ToString(), otherElement.Value, "Element is last added pair: value");
            
            subscription.Dispose();
            
            testNumberStringCollection[NumberEnum.Three] = "Tiga";
            Assert.AreEqual(NumberEnum.Seven, element.Key, "Should be unchanged due to disposal.");
            Assert.AreEqual(NumberEnum.Seven.ToString(), element.Value, "Should be unchanged due to disposal.");
            Assert.AreEqual(NumberEnum.Three, otherElement.Key, "Other Element with key Three changed: key.");
            Assert.AreEqual("Tiga", otherElement.Value, "Other Element with key Three changed: value.");
            
            AssertCollectionContents();
        }
        
        [Test]
        public void DictionaryPublicMethods_ShouldBeCalled()
        {
            testNumberStringCollection.Clear();
            testNumberStringCollection.Add(NumberEnum.One, NumberEnum.One.ToString());
            testNumberStringCollection.Add(NumberEnum.Two, NumberEnum.Two.ToString());
            testNumberStringCollection.Add(NumberEnum.Three, NumberEnum.Three.ToString());
            testNumberStringCollection.Add(NumberEnum.Four, NumberEnum.Four.ToString());
            testNumberStringCollection.Add(NumberEnum.Ten, NumberEnum.Ten.ToString());
            
            // Contains
            Assert.IsTrue(testNumberStringCollection.Contains(new KeyValuePair<NumberEnum, string>(NumberEnum.Three, NumberEnum.Three.ToString())), "Contains Key Three.");
            Assert.IsFalse(testNumberStringCollection.Contains(new KeyValuePair<NumberEnum, string>(NumberEnum.Five, NumberEnum.Five.ToString())), "Does not Contains Key Five.");
            
            // ContainsKey 
            Assert.IsTrue(testNumberStringCollection.ContainsKey(NumberEnum.Four), "Contains Key Four.");
            Assert.IsFalse(testNumberStringCollection.ContainsKey(NumberEnum.Zero), "Does not Contains Key Zero.");
            
            // TryGetValue
            Assert.IsFalse(testNumberStringCollection.TryGetValue(NumberEnum.Eight, out var eightValue), "Try Get Key Eight.");
            Assert.IsTrue(testNumberStringCollection.TryGetValue(NumberEnum.Ten, out var tenValue), "Try Get Key Ten.");
            Assert.IsNull(eightValue, "Value of Key Eight is null due to TryGetValue unsuccessful.");
            Assert.AreEqual(NumberEnum.Ten.ToString(), tenValue, "Value of Key Ten.");
            
            // Keys and Values
            var keys = testNumberStringCollection.Keys;
            var values = testNumberStringCollection.Values;
            Assert.AreEqual(NumberEnum.One, keys.First(), "First key is One.");
            Assert.AreEqual(NumberEnum.Ten.ToString(), values.Last(), "Last value is Ten.");
            
            // Copy
            var otherCollection = new[]
            {
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.One, NumberEnum.One.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Three, NumberEnum.Three.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Five, NumberEnum.Five.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Seven, NumberEnum.Seven.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Nine, NumberEnum.Nine.ToString())
            };
            testNumberStringCollection.Copy(otherCollection);
            Assert.AreEqual(testNumberStringCollection[NumberEnum.One], NumberEnum.One.ToString(), "Copy other collection.");
            Assert.AreEqual(testNumberStringCollection[NumberEnum.Three], NumberEnum.Three.ToString(), "Copy other collection.");
            Assert.AreEqual(testNumberStringCollection[NumberEnum.Five], NumberEnum.Five.ToString(), "Copy other collection.");
            Assert.AreEqual(testNumberStringCollection[NumberEnum.Seven], NumberEnum.Seven.ToString(), "Copy other collection.");
            Assert.AreEqual(testNumberStringCollection[NumberEnum.Nine], NumberEnum.Nine.ToString(), "Copy other collection.");
            
            // CopyTo
            var copyToCollection = new KeyValuePair<NumberEnum, string>[5];
            testNumberStringCollection.CopyTo(copyToCollection, 0);
            Assert.AreEqual(copyToCollection[0].Key, NumberEnum.One, "Copy To: Key One.");
            Assert.AreEqual(copyToCollection[1].Key, NumberEnum.Three, "Copy To: Key Three.");
            Assert.AreEqual(copyToCollection[2].Value, NumberEnum.Five.ToString(), "Copy To: Key Five's value.");
            Assert.AreEqual(copyToCollection[3].Value, NumberEnum.Seven.ToString(), "Copy To: Key Seven's value.");
            Assert.AreEqual(copyToCollection[4], new KeyValuePair<NumberEnum, string>(NumberEnum.Nine, NumberEnum.Nine.ToString()), "Copy To: KeyValuePair Nine.");

            // Copied members equality
            for (var i = 0; i < testNumberStringCollection.Count; i++)
            {
                var key = (NumberEnum)(i * 2 + 1);
                Assert.AreEqual(otherCollection[i].Value, testNumberStringCollection[key], "Copied collection value should be equal.");
                Assert.AreEqual(copyToCollection[i].Value, testNumberStringCollection[key], "Collection copied into should have equal value.");
            }
            
            AssertCollectionContents();
        }
        
        [Test]
        public void DictionaryPublicMethods_FromBaseClass_ShouldBeCalled()
        {
            testNumberStringCollection.Clear();
            testNumberStringCollection.Add(NumberEnum.Two, NumberEnum.Two.ToString());
            testNumberStringCollection.Add(NumberEnum.Ten, NumberEnum.Ten.ToString());
            testNumberStringCollection.Add(NumberEnum.Four, NumberEnum.Four.ToString());
            AssertCollectionContents();
            
            // Move
            testNumberStringCollection.Move(1, 2);
            AssertCollectionContents();
            
            // Insert
            testNumberStringCollection.Insert(0, new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.One, NumberEnum.One.ToString()));
            testNumberStringCollection.Insert(2, new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Three, NumberEnum.Three.ToString()));
            AssertCollectionContents();
            
            // InsertRange
            testNumberStringCollection.InsertRange(4, Enumerable.Range(1, 5).Select(ConvertFromIndex));
            AssertCollectionContents();
            
            // RemoveAt
            testNumberStringCollection.RemoveAt(0);
            AssertCollectionContents();
            
            SerializedKeyValuePair<NumberEnum, string> ConvertFromIndex(int val)
            {
                var key = (NumberEnum)(val + 4);
                var value = key.ToString();
                return new SerializedKeyValuePair<NumberEnum, string>(key, value);
            }
        }

        private void AssertCollectionContents()
        {
            IList<SerializedKeyValuePair<NumberEnum, string>> asList = testNumberStringCollection;
            IDictionary<NumberEnum, string> asDictionary = testNumberStringCollection;
            
            for (var i = 0; i < asList.Count; i++)
            {
                var pair = testNumberStringCollection[i];
                Assert.IsTrue(testNumberStringCollection.TryGetValue(pair.Key, out var value), "TryGetValue should be successful.");
                Assert.AreEqual(pair.Value, value, "Value should be equal.");
            }
            
            foreach (var key in asDictionary.Keys)
            {
                Assert.IsTrue(asList.Any(pair => pair.Key.Equals(key) && pair.Value.Equals(asDictionary[key])), "Key should be found in list and value should be equal.");
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testNumberStringCollection);
        }
    }
}