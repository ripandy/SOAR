using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Soar.Collections.Tests
{
    // TODO: Some test messages need fixing.
    public partial class CollectionListTests
    {
        private IntCollection testIntCollection;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            testIntCollection = ScriptableObject.CreateInstance<IntCollection>();
        }

        [Test]
        public void SubscribeOnAdd_ShouldBeListened()
        {
            testIntCollection.Clear();
            
            var addedValues = new System.Collections.Generic.List<int>();
            var subscription = testIntCollection.SubscribeOnAdd(addedVal => addedValues.Add(addedVal));

            testIntCollection.Add(42);
            Assert.AreEqual(42, addedValues[0], "Simple add.");
            
            testIntCollection.Add(1);
            testIntCollection.Add(2);
            testIntCollection.Add(3);
            Assert.AreEqual(1, addedValues[1], "Add multiple times.");
            Assert.AreEqual(2, addedValues[2], "Add multiple times.");
            Assert.AreEqual(3, addedValues[3], "Add multiple times.");
            
            Assert.AreEqual(4, addedValues.Count, "Event count before disposal.");
            subscription.Dispose();
            
            testIntCollection.Add(10);
            Assert.AreEqual(4, addedValues.Count, "Event count after disposal should not be changed.");
        }
        
        [Test]
        public void SubscribeOnAdd_ByAddRange_ShouldBeListened()
        {
            testIntCollection.Clear();
            
            var addedValues = new System.Collections.Generic.List<int>();
            using var subscription = testIntCollection.SubscribeOnAdd(addedVal => addedValues.Add(addedVal));
            
            testIntCollection.AddRange(new[] { 7, 8, 9 });
            Assert.AreEqual(7, addedValues[0], "Add range.");
            Assert.AreEqual(8, addedValues[1], "Add range.");
            Assert.AreEqual(9, addedValues[2], "Add range.");
            
            testIntCollection.AddRange(Enumerable.Range(0, 3).Select((_, index) => index * 2));
            Assert.AreEqual(0, addedValues[3], "Add enumerable range.");
            Assert.AreEqual(2, addedValues[4], "Add enumerable range.");
            Assert.AreEqual(4, addedValues[5], "Add enumerable range.");
        }
        
        [Test]
        public void SubscribeOnAdd_ByInsert_ShouldBeListened()
        {
            testIntCollection.Clear();
            testIntCollection.AddRange(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            
            var addedValues = new System.Collections.Generic.List<int>();
            using var subscription = testIntCollection.SubscribeOnAdd(addedVal => addedValues.Add(addedVal));

            testIntCollection.Insert(1, 24);
            Assert.AreEqual(24, addedValues[0], "Simple insert.");
            
            testIntCollection.Insert(5, 42);
            testIntCollection.Insert(0, 0);
            testIntCollection.Insert(3, 12);
            Assert.AreEqual(42, addedValues[1], "Simple insert.");
            Assert.AreEqual(0, addedValues[2], "Simple insert.");
            Assert.AreEqual(12, addedValues[3], "Simple insert.");
        }
        
        [Test]
        public void SubscribeOnAdd_ByInsertRange_ShouldBeListened()
        {
            testIntCollection.Clear();
            testIntCollection.AddRange(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            
            var addedValues = new System.Collections.Generic.List<int>();
            using var subscription = testIntCollection.SubscribeOnAdd(addedVal => addedValues.Add(addedVal));

            testIntCollection.InsertRange(5, new[] { 4, 5, 6 });
            Assert.AreEqual(4, addedValues[0], "Insert range.");
            Assert.AreEqual(5, addedValues[1], "Insert range.");
            Assert.AreEqual(6, addedValues[2], "Insert range.");

            testIntCollection.InsertRange(0, Enumerable.Range(0, 3).Select((_, index) => index * 2));
            Assert.AreEqual(0, addedValues[3], "Insert enumerable range.");
            Assert.AreEqual(2, addedValues[4], "Insert enumerable range.");
            Assert.AreEqual(4, addedValues[5], "Insert enumerable range.");
        }
        
        [Test]
        public void SubscribeOnRemove_ShouldBeListened()
        {
            testIntCollection.Clear();
            testIntCollection.AddRange(new [] { 1, 2, 3, 4, 24, 5, 6, 7, 8, 9 });
            
            var removedValue = 0;
            var subscription = testIntCollection.SubscribeOnRemove(removedVal => removedValue = removedVal);

            var removed = testIntCollection.Remove(24);
            Assert.AreEqual(24, removedValue, "Remove by value.");
            Assert.IsTrue(removed, "Removed is true.");
            
            removed = testIntCollection.Remove(42);
            Assert.AreEqual(24, removedValue, "Should not be updated due to removal failure (doesn't exist).");
            Assert.IsFalse(removed, "Removed is false.");
            
            testIntCollection.RemoveAt(0);
            Assert.AreEqual(1, removedValue, "Remove at index.");
            
            subscription.Dispose();
            
            testIntCollection.RemoveAt(5);
            Assert.AreEqual(1, removedValue, "Should not be updated due to subscription has been disposed");
        }
        
        [Test]
        public void SubscribeOnClear_ShouldBeListened()
        {
            testIntCollection.Clear();
            
            var cleared = false;
            var subscription = testIntCollection.SubscribeOnClear(() => cleared = true);
            Assert.IsFalse(cleared, "Should not be updated due to subscription happened later.");
            
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
            Assert.IsFalse(cleared, "Should not be updated due to subscription has been disposed.");
        }
        
        [Test]
        public void SubscribeToCount_ShouldBeListened()
        {
            testIntCollection.Clear();
            
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
        
        [Test]
        public void SubscribeToValues_ShouldBeListened()
        {
            testIntCollection.Clear();
            testIntCollection.AddRange(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            
            var elements = new System.Collections.Generic.List<IndexValuePair<int>>();
            var subscription = testIntCollection.SubscribeToValues(value => elements.Add(value));

            testIntCollection[1] = 24;
            Assert.AreEqual(1, elements[0].Index, "Element at index 1 updated.");
            Assert.AreEqual(24, elements[0].Value, "Element value should be updated to 24.");
            
            testIntCollection[0] = 83;
            testIntCollection[8] = 84;
            Assert.AreEqual(0, elements[1].Index, "Element at index 0 updated.");
            Assert.AreEqual(83, elements[1].Value, "Element value should be updated to 83.");
            Assert.AreEqual(8, elements[2].Index, "Element at index 8 updated.");
            Assert.AreEqual(84, elements[2].Value, "Element value should be updated to 84.");
            
            var otherElements = new System.Collections.Generic.List<(int Index, int Value)>();
            using var subscription2 = testIntCollection.SubscribeToValues((index, value) => otherElements.Add((index, value)));
            
            testIntCollection[5] = 42;
            Assert.AreEqual(5, elements[3].Index, "Element at index 5 updated.");
            Assert.AreEqual(42, elements[3].Value, "Element value should be updated to 42.");
            Assert.AreEqual(5, otherElements[0].Index, "Element at index 5 updated.");
            Assert.AreEqual(42, otherElements[0].Value, "Element value should be updated to 42.");
            
            Assert.AreEqual(4, elements.Count, "Element count before disposal should be 4.");
            subscription.Dispose();
            
            testIntCollection[3] = 30;
            Assert.AreEqual(4, elements.Count, "Element count should stay at 4 due to disposal.");
            Assert.AreEqual(3, otherElements[1].Index, "Element at index 3 changed.");
            Assert.AreEqual(30, otherElements[1].Value, "Element value should be updated to 42.");
        }

        [Test]
        public void SubscribeToValues_ByAdd_ShouldBeListened()
        {
            testIntCollection.Clear();
            
            var elements = new System.Collections.Generic.List<IndexValuePair<int>>();
            using var subscription = testIntCollection.SubscribeToValues(value => elements.Add(value));

            testIntCollection.Add(1);
            testIntCollection.Add(5);
            testIntCollection.Add(7);
            Assert.AreEqual(0, elements[0].Index, "Element at index 0 added.");
            Assert.AreEqual(1, elements[0].Value, "Element value at index 0 should be 1.");
            Assert.AreEqual(1, elements[1].Index, "Element at index 1 added.");
            Assert.AreEqual(5, elements[1].Value, "Element value at index 1 should be 5.");
            Assert.AreEqual(2, elements[2].Index, "Element at index 2 added.");
            Assert.AreEqual(7, elements[2].Value, "Element value at index 2 should be 7.");
        }

        [Test]
        public void SubscribeToValues_ByAddRange_ShouldBeListened()
        {
            testIntCollection.Clear();
            
            var elements = new System.Collections.Generic.List<IndexValuePair<int>>();
            using var subscription = testIntCollection.SubscribeToValues(value => elements.Add(value));

            testIntCollection.AddRange(new[] { 7, 8, 9 });
            Assert.AreEqual(new IndexValuePair<int>(0, 7), elements[0], "Element at index 0 added with value of 7.");
            Assert.AreEqual(new IndexValuePair<int>(1, 8), elements[1], "Element at index 1 added with value of 8.");
            Assert.AreEqual(new IndexValuePair<int>(2, 9), elements[2], "Element at index 2 added with value of 9.");
            
            testIntCollection.AddRange(Enumerable.Range(0, 3).Select((_, index) => index * 2));
            Assert.AreEqual(new IndexValuePair<int>(3, 0), elements[3], "Element at index 3 added with value of 0.");
            Assert.AreEqual(new IndexValuePair<int>(4, 2), elements[4], "Element at index 4 added with value of 2.");
            Assert.AreEqual(new IndexValuePair<int>(5, 4), elements[5], "Element at index 5 added with value of 4.");
        }

        [Test]
        public void SubscribeToValues_ByInsert_ShouldBeListened()
        {
            testIntCollection.Clear();
            testIntCollection.AddRange(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            
            var elements = new System.Collections.Generic.List<IndexValuePair<int>>();
            using var subscription = testIntCollection.SubscribeToValues(value => elements.Add(value));
            
            testIntCollection.Insert(1, 10);
            Assert.AreEqual(1, elements[0].Index, "Element at index 1 inserted.");
            Assert.AreEqual(10, elements[0].Value, "Element value should be 10.");
            
            testIntCollection.Insert(5, 42);
            testIntCollection.Insert(0, 0);
            testIntCollection.Insert(3, 12);
            Assert.AreEqual(5, elements[1].Index, "Element at index 5 inserted.");
            Assert.AreEqual(42, elements[1].Value, "Element value should be 42.");
            Assert.AreEqual(0, elements[2].Index, "Element at index 0 inserted.");
            Assert.AreEqual(0, elements[2].Value, "Element value should be 0.");
            Assert.AreEqual(3, elements[3].Index, "Element at index 3 inserted.");
            Assert.AreEqual(12, elements[3].Value, "Element value should be 12.");
        }
        
        [Test]
        public void SubscribeToValues_ByInsertRange_ShouldBeListened()
        {
            testIntCollection.Clear();
            testIntCollection.AddRange(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            
            var elements = new System.Collections.Generic.List<IndexValuePair<int>>();
            using var subscription = testIntCollection.SubscribeToValues(value => elements.Add(value));
            
            testIntCollection.InsertRange(5, new[] { 4, 5, 6 });
            Assert.AreEqual(new IndexValuePair<int>(5, 4), elements[0], "Element at index 5 inserted with value of 4.");
            Assert.AreEqual(new IndexValuePair<int>(6, 5), elements[1], "Element at index 6 inserted with value of 5.");
            Assert.AreEqual(new IndexValuePair<int>(7, 6), elements[2], "Element at index 7 inserted with value of 6.");
            
            testIntCollection.InsertRange(0, Enumerable.Range(0, 3).Select((_, index) => index * 2));
            Assert.AreEqual(new IndexValuePair<int>(0, 0), elements[3], "Element at index 0 inserted with value of 0.");
            Assert.AreEqual(new IndexValuePair<int>(1, 2), elements[4], "Element at index 1 inserted with value of 2.");
            Assert.AreEqual(new IndexValuePair<int>(2, 4), elements[5], "Element at index 2 inserted with value of 4.");
        }
        
        [Test]
        public void SubscribeOnMove_ShouldBeListened()
        {
            testIntCollection.Clear();
            testIntCollection.AddRange(new [] { 1, 2, 3, 42, 5, 6, 7, 8, 9 });
            
            var elements = new System.Collections.Generic.List<MovedValueDto<int>>();
            var subscription = testIntCollection.SubscribeOnMove(value => elements.Add(value));

            testIntCollection.Move(3, 0);
            Assert.AreEqual(3, elements[0].OldIndex, "Old index should be 3.");
            Assert.AreEqual(0, elements[0].NewIndex, "New index should be 0.");
            Assert.AreEqual(42, elements[0].Value, "Moved element value should be 42.");

            var otherElements = new System.Collections.Generic.List<(int Value, int OldIndex, int NewIndex)>();
            using var subscription2 = testIntCollection.SubscribeOnMove((value, prevIndex, newIndex) => otherElements.Add((value, prevIndex, newIndex)));
            
            testIntCollection.Move(8, 0);
            Assert.AreEqual(8, elements[1].OldIndex, "Old index should be 8.");
            Assert.AreEqual(0, elements[1].NewIndex, "New index should be 0.");
            Assert.AreEqual(9, elements[1].Value, "Moved element value should be 9.");
            Assert.AreEqual(8, otherElements[0].OldIndex, "Old index should be 8.");
            Assert.AreEqual(0, otherElements[0].NewIndex, "New index should be 0.");
            Assert.AreEqual(9, otherElements[0].Value, "Moved element value should be 9.");
            Assert.AreEqual(1, testIntCollection.IndexOf(42), "Index of 42 should be 1.");
            
            Assert.AreEqual(2, elements.Count, "Element count before disposal should be 2.");
            subscription.Dispose();
            
            testIntCollection.Move(2, 8);
            Assert.AreEqual(2, elements.Count, "Element count should stay at 2 due to disposal.");
            Assert.AreEqual(2, otherElements[1].OldIndex, "Old index should be 2.");
            Assert.AreEqual(8, otherElements[1].NewIndex, "New index should be 8.");
            Assert.AreEqual(1, otherElements[1].Value, "Moved element value should be 1.");
        }
        
        [Test]
        public void ListPublicMethods_ShouldBeCalled()
        {
            testIntCollection.Clear();
            testIntCollection.AddRange(new [] { 1, 2, 3, 4, 42, 5, 6, 7, 8, 9 });
            Assert.IsTrue(testIntCollection.Contains(42), "Contains 42.");
            Assert.IsFalse(testIntCollection.Contains(24), "Does not Contains 24.");
            
            var sum = 0;
            testIntCollection.ForEach(value => sum += value);
            Assert.AreEqual(87, sum, "Sum of all elements using ForEach method.");
            
            Assert.AreEqual(4, testIntCollection.IndexOf(42), "Index of 42.");
            
            testIntCollection.Move(4, 0);
            Assert.AreEqual(4, testIntCollection[4], "Move 0 to 4.");
            Assert.AreEqual(42, testIntCollection[0], "Move 4 to 0.");
            
            var otherCollection = new int[10];
            testIntCollection.CopyTo(otherCollection, 0);
            Assert.AreEqual(1, otherCollection[1], "Copied to other collection.");
            
            var modifiedOtherCollection = otherCollection.Select(value => value + 10).ToArray();
            testIntCollection.Copy(modifiedOtherCollection);
            Assert.AreEqual(52, testIntCollection[0], "Copied from other collection.");
            Assert.AreEqual(11, testIntCollection[1], "Copied from other collection.");
            
            testIntCollection.ResetValues();
            Assert.AreEqual(0, testIntCollection.Count, "Initial value assumed to be empty.");
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testIntCollection);
        }
    }
}