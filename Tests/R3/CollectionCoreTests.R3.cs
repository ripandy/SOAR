#if SOAR_R3

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Soar.Collections.Tests
{
    public partial class CollectionCoreListTests
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

        [Test]
        public void ObservableIntegration_ShouldBeConverted()
        {
            // TODO: Implement ObservableIntegration_ShouldBeConverted
        }
    }

    public partial class CollectionCoreDictionaryTests
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
        
        [Test]
        public void ObservableIntegration_ShouldBeConverted()
        {
            // TODO: Implement ObservableIntegration_ShouldBeConverted
        }
    }
}

#endif