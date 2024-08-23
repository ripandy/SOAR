using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Soar.Collections.Tests
{
    public class CollectionCoreTests
    {
        private IntCollection testIntCollection;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            testIntCollection = ScriptableObject.CreateInstance<IntCollection>();
        }

        [SetUp]
        public void Setup()
        {
            testIntCollection.Clear();
        }

        [Test]
        public void SubscribeOnAdd_ShouldBeListened()
        {
            var addedValue = 0;
            var subscription = testIntCollection.SubscribeOnAdd(addedVal => addedValue = addedVal);

            testIntCollection.Add(42);
            Assert.AreEqual(42, addedValue, "Simple add.");
            
            testIntCollection.Add(1);
            testIntCollection.Add(2);
            testIntCollection.Add(3);
            Assert.AreEqual(3, addedValue, "Add multiple times.");
            
            testIntCollection.AddRange(new[] { 7, 8, 9 });
            Assert.AreEqual(9, addedValue, "Add range.");
            
            testIntCollection.AddRange(Enumerable.Range(0, 3).Select((_, index) => index * 2));
            Assert.AreEqual(4, addedValue, "Add enumerable range.");
            
            testIntCollection.Insert(1, 24);
            Assert.AreEqual(24, addedValue, "Simple insert.");
            
            testIntCollection.InsertRange(5, new[] { 4, 5, 6 });
            Assert.AreEqual(6, addedValue, "Insert range.");

            var count = testIntCollection.Count;
            var expected = count + 2;
            testIntCollection.InsertRange(count, Enumerable.Range(0, 3).Select((_, index) => index + count));
            Assert.AreEqual(expected, addedValue, "Insert enumerable range.");
            
            subscription.Dispose();
            
            testIntCollection.Add(10);
            Assert.AreEqual(expected, addedValue, "Should not be updated due to subscription has been disposed");
        }
        
        [Test]
        public void SubscribeOnRemove_ShouldBeListened()
        {
            var removedValue = 0;
            var subscription = testIntCollection.SubscribeOnRemove(removedVal => removedValue = removedVal);
            
            testIntCollection.AddRange(new [] { 1, 2, 3, 4, 24, 5, 6, 7, 8, 9 });
            
            testIntCollection.Remove(24);
            Assert.AreEqual(24, removedValue, "Remove by value.");
            
            testIntCollection.RemoveAt(0);
            Assert.AreEqual(1, removedValue, "Remove at index.");
            
            subscription.Dispose();
            
            testIntCollection.RemoveAt(5);
            Assert.AreEqual(1, removedValue, "Should not be updated due to subscription has been disposed");
        }
        
        [Test]
        public void SubscribeOnClear_ShouldBeListened()
        {
            var cleared = false;
            var subscription = testIntCollection.SubscribeOnClear(() => cleared = true);
            
            testIntCollection.Add(1);
            testIntCollection.Add(2);
            testIntCollection.Add(42);
            testIntCollection.Add(3);
            testIntCollection.Clear();
            
            Assert.IsTrue(cleared, "Clear collection.");

            cleared = false;
            subscription.Dispose();
            
            testIntCollection.Add(4);
            testIntCollection.Add(5);
            testIntCollection.Add(6);
            testIntCollection.Clear();
            Assert.IsFalse(cleared, "Should not be updated due to subscription has been disposed");
        }
        
        [Test]
        public void SubscribeToCount_ShouldBeListened()
        {
            var countValue = 0;
            var subscription = testIntCollection.SubscribeToCount(count => countValue = count);
            
            testIntCollection.Add(1);
            testIntCollection.Add(2);
            testIntCollection.Add(42);
            testIntCollection.Add(3);
            Assert.AreEqual(4, countValue, "Added 4 times.");
            
            testIntCollection.Remove(42);
            Assert.AreEqual(3, countValue, "Removed 1 time from count = 4.");
            
            testIntCollection.Clear();
            Assert.AreEqual(0, countValue, "Cleared collection.");
            
            subscription.Dispose();
            
            testIntCollection.Add(4);
            testIntCollection.Add(5);
            testIntCollection.Add(6);
            Assert.AreEqual(0, countValue, "Should not be updated due to subscription has been disposed");
        }
        
        private readonly struct Element
        {
            public readonly int Index;
            public readonly int Value;
            
            public Element(int index, int value)
            {
                Index = index;
                Value = value;
            }
        }
        
        [Test]
        public void SubscribeToValues_ShouldBeListened()
        {
            testIntCollection.Add(1);
            testIntCollection.Add(2);
            testIntCollection.Add(42);
            testIntCollection.Add(3);
            
            var element = new Element();
            
            var subscription = testIntCollection.SubscribeToValues(SetElement);
            
            Assert.AreEqual(0, element.Index, "Should be default index since subscription happened late.");
            Assert.AreEqual(0, element.Value, "Should be default value since subscription happened late.");

            testIntCollection[1] = 24;
            Assert.AreEqual(1, element.Index, "Element at index 1 changed.");
            Assert.AreEqual(24, element.Value, "Element is changed.");
            
            testIntCollection.RemoveAt(0);
            
            testIntCollection[0] = 83;
            testIntCollection[2] = 84;
            Assert.AreEqual(2, element.Index, "Element at index 2 changed.");
            Assert.AreEqual(84, element.Value, "Element is changed.");
            
            testIntCollection.Add(1);
            testIntCollection.Add(5);
            testIntCollection.Add(7);
            var lastElementIndex = testIntCollection.Count - 1;
            Assert.AreEqual(lastElementIndex, element.Index, "Element at last index changed.");
            Assert.AreEqual(7, element.Value, "Element value is the last added value.");
            
            var otherElement = new Element();
            using var subscription2 = testIntCollection.SubscribeToValues(SetOtherElement);
            
            testIntCollection.Insert(1, 10);
            Assert.AreEqual(1, element.Index, "Element at index 1 inserted.");
            Assert.AreEqual(10, element.Value, "Element value should be the inserted value.");
            Assert.AreEqual(1, otherElement.Index, "Element at index 1 inserted.");
            Assert.AreEqual(10, otherElement.Value, "Element value should be the inserted value.");
            
            subscription.Dispose();
            
            testIntCollection[3] = 30;
            Assert.AreEqual(1, element.Index, "Should be unchanged due to disposal.");
            Assert.AreEqual(10, element.Value, "Should be unchanged due to disposal.");
            Assert.AreEqual(3, otherElement.Index, "Element at index 3 changed.");
            Assert.AreEqual(30, otherElement.Value, "Element value should be changed.");
            
            testIntCollection.AddRange(new[] { 7, 8, 9 });
            lastElementIndex = testIntCollection.Count - 1;
            Assert.AreEqual(lastElementIndex, otherElement.Index, "Element at last index changed.");
            Assert.AreEqual(9, otherElement.Value, "Element value is the last added value.");
            
            testIntCollection.AddRange(Enumerable.Range(0, 3).Select((_, index) => index * 2));
            lastElementIndex = testIntCollection.Count - 1;
            Assert.AreEqual(lastElementIndex, otherElement.Index, "Element at last index changed.");
            Assert.AreEqual(4, otherElement.Value, "Element value is the last added value.");
            
            testIntCollection.InsertRange(5, new[] { 4, 5, 6 });
            Assert.AreEqual(7, otherElement.Index, "Element at index 5-7 inserted.");
            Assert.AreEqual(6, otherElement.Value, "Element value should be the last inserted value.");
            
            void SetElement(int index, int value)
            {
                element = new Element(index, value);
            }
            
            void SetOtherElement(int index, int value)
            {
                otherElement = new Element(index, value);
            }
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            Object.Destroy(testIntCollection);
        }
    }
}