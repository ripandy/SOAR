#if SOAR_R3

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Soar.Collections.Tests
{
    public partial class CollectionListTests
    {
        [Test]
        public async Task EventAsync_ShouldBeAwaited()
        {
            testIntCollection.Clear();
            testIntCollection.AddRange(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            
            var addTask = testIntCollection.OnAddAsync();
            testIntCollection.Add(42);
            var eventValue = await addTask;
            Assert.AreEqual(42, eventValue);
            
            var removeTask = testIntCollection.OnRemoveAsync();
            testIntCollection.Remove(4);
            eventValue = await removeTask;
            Assert.AreEqual(4, eventValue);
            
            var clearTask = testIntCollection.OnClearAsync();
            testIntCollection.Clear();
            await clearTask;
            
            var countTask = testIntCollection.CountAsync();
            testIntCollection.AddRange(new [] { 1, 2, 3 });
            var count = await countTask;
            Assert.AreEqual(3, count);
            
            var valuesTask = testIntCollection.ValuesAsync();
            testIntCollection[2] = 24;
            var values = await valuesTask;
            Assert.AreEqual(new IndexValuePair<int>(2, 24), values);
            
            var valueTaskAt = testIntCollection.ValuesAsync(0);
            testIntCollection[0] = 42;
            var valueAt = await valueTaskAt;
            Assert.AreEqual(42, valueAt);

            var moveValueTask = testIntCollection.OnMoveAsync();
            testIntCollection.Move(0, 1);
            var movedValue = await moveValueTask;
            Assert.AreEqual(new MovedValueDto<int>(42, 0, 1), movedValue);
        }

        [Test]
        public void EventAsync_ShouldThrowIfCancelled()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await testIntCollection.OnAddAsync(cts.Token));
            Assert.ThrowsAsync<TaskCanceledException>(async () => await testIntCollection.OnRemoveAsync(cts.Token));
            Assert.ThrowsAsync<TaskCanceledException>(async () => await testIntCollection.OnClearAsync(cts.Token));
            Assert.ThrowsAsync<TaskCanceledException>(async () => await testIntCollection.CountAsync(cts.Token));
            Assert.ThrowsAsync<TaskCanceledException>(async () => await testIntCollection.ValuesAsync(cts.Token));
        }
    }

    public partial class CollectionDictionaryTests
    {
        [Test]
        public async Task EventAsync_ShouldBeAwaited()
        {
            testNumberStringCollection.Clear();
            testNumberStringCollection.AddRange(new []
            {
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.One, NumberEnum.One.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Three, NumberEnum.Three.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Five, NumberEnum.Five.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Seven, NumberEnum.Seven.ToString()),
                new SerializedKeyValuePair<NumberEnum, string>(NumberEnum.Nine, NumberEnum.Nine.ToString())
            });

            var valuesTask = testNumberStringCollection.ValuesAsync();
            testNumberStringCollection[NumberEnum.Three] = "Tiga";
            var values = await valuesTask;
            Assert.AreEqual(new KeyValuePair<NumberEnum, string>(NumberEnum.Three, "Tiga"), values);
        }

        [Test]
        public void EventAsync_ShouldThrowIfCancelled()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            
            Assert.ThrowsAsync<TaskCanceledException>(async () => await testNumberStringCollection.ValuesAsync(cts.Token));
        }
    }
}

#endif