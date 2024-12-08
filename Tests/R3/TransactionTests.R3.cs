#if SOAR_R3

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Soar.Transactions.Tests
{
    public partial class TransactionTests
    {
        private NumberToIntTransaction testNumberToIntTransaction;
        
        [OneTimeSetUp]
        public void OneTimeSetupR3()
        {
            testNumberToIntTransaction = ScriptableObject.CreateInstance<NumberToIntTransaction>();
        }

        [SetUp]
        public void SetupR3()
        {
            testNumberToIntTransaction.ResetResponseInternal();
            testNumberToIntTransaction.ClearRequests();
        }
        
        [TestCase(NumberEnum.Five)]
        [TestCase(NumberEnum.Seven)]
        [TestCase(NumberEnum.Nine)]
        public async Task ValueTransaction_RegisterResponseWithCancellationToken_ShouldBeCancelledAccordingly(
            NumberEnum requestValue)
        {
            var cts = new CancellationTokenSource();
            
            var responseValue = await testNumberToStringTransaction.RequestAsync(requestValue, cts.Token);
            Assert.AreEqual(requestValue.ToString(), responseValue);
            
            var requestTask = testNumberToStringTransaction.RequestAsync(requestValue, cts.Token);
            testNumberToStringTransaction.RegisterResponse(FromNumberEnumAsync);
            responseValue = await requestTask;
            Assert.AreEqual(requestValue.ToString(), responseValue);
            
            cts.Cancel();
            
            Assert.ThrowsAsync<TaskCanceledException>(async () => await testNumberToStringTransaction.RequestAsync(requestValue, cts.Token));
        }

        [TestCase(AwaitOperation.Sequential, 3)]
        [TestCase(AwaitOperation.Drop, 1)]
        [TestCase(AwaitOperation.Switch, 1)]
        [TestCase(AwaitOperation.Parallel, 3)]
        [TestCase(AwaitOperation.SequentialParallel, 3)]
        [TestCase(AwaitOperation.ThrottleFirstLast, 3)]
        public async Task PlainTransaction_RegisterResponse_WithAwaitOperation_ShouldBeRegistered(
            AwaitOperation awaitOperation, int expectedSuccessfulCount)
        {
            const int requestTimes = 3;
            const int betweenCallDelay = 200;
            const int defaultDelay = betweenCallDelay * requestTimes;
            
            var requestCount = 0;
            var successfulRequestCount = 0;
            var successfulResponseCount = 0;
            var cts = new CancellationTokenSource();
            
            testTransaction.RegisterResponse(ResponseAsync, awaitOperation);
            
            for (var i = 1; i <= requestTimes; i++)
            {
                RequestAsync(cts.Token);
                await Task.Delay(betweenCallDelay, cts.Token);
            }
            
            await Task.Delay((requestTimes + 1) * defaultDelay, cts.Token);
            await Task.Yield();
            
            Assert.IsTrue(requestTimes == requestCount, $"Request Count is not equal to {requestTimes}");
            // MEMO: AwaitOperation Drop and Switch has different result on successful Request and Response count.
            Assert.IsTrue(expectedSuccessfulCount == successfulRequestCount ||
                          expectedSuccessfulCount == successfulResponseCount,
                $"neither successfulRequestCount:{successfulRequestCount} or successfulResponseCount{successfulResponseCount} is equal to {expectedSuccessfulCount}");
            
            cts.Cancel();

            await Task.Yield();
            
            async void RequestAsync(CancellationToken token)
            {
                try
                {
                    requestCount++;
                    await testTransaction.RequestAsync(token).AsTask();
                    successfulRequestCount++;
                }
                catch (TaskCanceledException)
                {
                }
            }

            async ValueTask ResponseAsync(CancellationToken token)
            {
                try
                {
                    await Task.Delay(defaultDelay, token);
                    successfulResponseCount++;
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
        
        [TestCase(AwaitOperation.Sequential,  new[] { 2, 4, 6 })]
        [TestCase(AwaitOperation.Drop, new[] { 2 })]
        [TestCase(AwaitOperation.Switch, new[] { 6 })]
        [TestCase(AwaitOperation.Parallel, new[] { 2, 4, 6 })]
        [TestCase(AwaitOperation.SequentialParallel, new[] { 2, 4, 6 })]
        [TestCase(AwaitOperation.ThrottleFirstLast, new[] { 2, 4, 6 })]
        // MEMO: SequentialParallel and ThrottleFirstLast are not supported, and are being set to default Parallel.
        public async Task ValueTransaction_RegisterResponse_WithAwaitOperation_ShouldBeRegistered(
            AwaitOperation awaitOperation, int[] expectedValues)
        {
            const int requestTimes = 3;
            const int betweenCallDelay = 200;
            const int defaultDelay = betweenCallDelay * requestTimes;
            
            var requestCount = 0;
            var successfulRequestCount = 0;
            var successfulResponseCount = 0;
            var expectedSuccessfulCount = expectedValues.Length;
            var valueList = new List<int>();
            var cts = new CancellationTokenSource();

            testNumberToIntTransaction.RegisterResponse(ResponseAsync, awaitOperation);
            
            for (var i = 1; i <= requestTimes; i++)
            {
                RequestAsync((NumberEnum)i, cts.Token);
                await Task.Delay(betweenCallDelay, cts.Token);
            }
            
            await Task.Delay((requestTimes + 1) * defaultDelay, cts.Token);
            await Task.Yield();
            
            Assert.IsTrue(requestTimes == requestCount, $"Request Count is not equal to {requestTimes}");
            Assert.IsTrue(expectedSuccessfulCount == successfulRequestCount || 
                          expectedSuccessfulCount == successfulResponseCount,
                $"neither successfulRequestCount:{successfulRequestCount} or successfulResponseCount{successfulResponseCount} is equal to {expectedSuccessfulCount}");
            Assert.AreEqual(expectedValues, valueList.ToArray(), $"valueList: {string.Join(", ", valueList.Select(value => value.ToString()))} is not equal to {string.Join(", ", expectedValues.Select(value => value.ToString()))}");
            
            cts.Cancel();
            cts.Dispose();

            await Task.Yield();
            
            async void RequestAsync(NumberEnum number, CancellationToken token)
            {
                try
                {
                    requestCount++;
                    var responseValue = await testNumberToIntTransaction.RequestAsync(number, token).AsTask();
                    valueList.Add(responseValue);
                    successfulRequestCount++;
                }
                catch (TaskCanceledException)
                {
                }
            }

            async ValueTask<int> ResponseAsync(NumberEnum number, CancellationToken token)
            {
                await Task.Delay(defaultDelay, token);
                var result = (int)number * 2;
                successfulResponseCount++;
                return result;
            }
        }
        
        [Test]
        public async Task PlainTransaction_R3Integration_ToRequestAsyncEnumerable_ShouldBeConverted()
        {
            var cts = new CancellationTokenSource();
            var count = 0;
            
            testTransaction.RegisterResponse(ResponseAsync);
            
            var requestAsyncEnumerableTask = RunToRequestAsyncEnumerable();
            
            await testTransaction.RequestAsync(cts.Token);
            await testTransaction.RequestAsync(cts.Token);
            await testTransaction.RequestAsync(cts.Token);
            
            cts.Cancel();

            await requestAsyncEnumerableTask;
            
            Assert.AreEqual(3, count);

            async ValueTask RunToRequestAsyncEnumerable()
            {
                var startTime = Time.realtimeSinceStartup;
                var asyncEnumerable = testTransaction.ToRequestAsyncEnumerable(cts.Token);
                await foreach (var _ in asyncEnumerable)
                {
                    count++;
                }
                
                var elapsedTime = Time.realtimeSinceStartup - startTime;
                Assert.GreaterOrEqual(elapsedTime, 0.3f);
            }
            
            async ValueTask ResponseAsync(CancellationToken token)
            {
                await Task.Delay(100, token);
            }
        }
        
        [Test]
        public async Task PlainTransaction_R3Integration_ToResponseAsyncEnumerable_ShouldBeConverted()
        {
            var cts = new CancellationTokenSource();
            var count = 0;
            
            testTransaction.RegisterResponse(ResponseAsync);
            
            var responseAsyncEnumerableTask = RunToResponseAsyncEnumerable();
            
            await testTransaction.RequestAsync(cts.Token);
            await testTransaction.RequestAsync(cts.Token);
            await testTransaction.RequestAsync(cts.Token);
            
            cts.Cancel();

            await responseAsyncEnumerableTask;
            
            Assert.AreEqual(3, count);

            async ValueTask RunToResponseAsyncEnumerable()
            {
                var startTime = Time.realtimeSinceStartup;
                var asyncEnumerable = testTransaction.ToResponseAsyncEnumerable(cts.Token);
                await foreach (var _ in asyncEnumerable)
                {
                    count++;
                }
                
                var elapsedTime = Time.realtimeSinceStartup - startTime;
                Assert.GreaterOrEqual(elapsedTime, 0.3f);
            }
            
            async ValueTask ResponseAsync(CancellationToken token)
            {
                await Task.Delay(100, token);
            }
        }

        [Test]
        public async Task ValueTransaction_R3Integration_ToRequestAsyncEnumerable_ShouldBeConverted()
        {
            var cts = new CancellationTokenSource();
            var valueList = new List<NumberEnum>();
            
            var requestAsyncEnumerableTask = RunToRequestAsyncEnumerable();
            
            var responseValue = await testNumberToIntTransaction.RequestAsync(NumberEnum.Ten, cts.Token);
            Assert.AreEqual(10, responseValue);
            
            responseValue = await testNumberToIntTransaction.RequestAsync(NumberEnum.Eight, cts.Token);
            Assert.AreEqual(8, responseValue);
            
            responseValue = await testNumberToIntTransaction.RequestAsync(NumberEnum.Six, cts.Token);
            Assert.AreEqual(6, responseValue);
            
            cts.Cancel();

            await requestAsyncEnumerableTask;
            
            Assert.AreEqual(3, valueList.Count);

            async ValueTask RunToRequestAsyncEnumerable()
            {
                var asyncEnumerable = testNumberToIntTransaction.ToRequestAsyncEnumerable(cts.Token);
                await foreach (var value in asyncEnumerable)
                {
                    valueList.Add(value);
                }

                Assert.AreEqual(new[] { NumberEnum.Ten, NumberEnum.Eight, NumberEnum.Six }, valueList.ToArray());
            }
        }
        
        [Test]
        public async Task ValueTransaction_R3Integration_ToResponseAsyncEnumerable_ShouldBeConverted()
        {
            var cts = new CancellationTokenSource();
            var valueList = new List<NumberEnum>();
            
            var responseAsyncEnumerableTask = RunToResponseAsyncEnumerable();
            
            var responseValue = await testNumberToIntTransaction.RequestAsync(NumberEnum.Ten, cts.Token);
            Assert.AreEqual(10, responseValue);
            
            responseValue = await testNumberToIntTransaction.RequestAsync(NumberEnum.Eight, cts.Token);
            Assert.AreEqual(8, responseValue);
            
            responseValue = await testNumberToIntTransaction.RequestAsync(NumberEnum.Six, cts.Token);
            Assert.AreEqual(6, responseValue);
            
            cts.Cancel();

            await responseAsyncEnumerableTask;
            
            Assert.AreEqual(3, valueList.Count);
            
            async ValueTask RunToResponseAsyncEnumerable()
            {
                var asyncEnumerable = testNumberToIntTransaction.ToResponseAsyncEnumerable(cts.Token);
                await foreach (var value in asyncEnumerable)
                {
                    valueList.Add(value);
                }
                
                Assert.AreEqual(new[] { 10, 8, 6 }, valueList.ToArray());
            }
        }
        
        [Test]
        public async Task ValueTransaction_WaitForRequestAsync_ShouldBeAwaited()
        {
            var cts = new CancellationTokenSource();
            
            var requestValueTask = testNumberToIntTransaction.WaitForRequestAsync(cts.Token);

            RequestDelayed();

            var requestValue = await requestValueTask;
            
            Assert.AreEqual(NumberEnum.Five, requestValue);

            async void RequestDelayed()
            {
                try
                {
                    await Task.Delay(100, cts.Token);
                    var responseValue = await testNumberToIntTransaction.RequestAsync(NumberEnum.Five, cts.Token);
                    Assert.AreEqual(5, responseValue);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
        
        [Test]
        public async Task ValueTransaction_WaitForResponseAsync_ShouldBeAwaited()
        {
            var cts = new CancellationTokenSource();
            
            var responseValueTask = testNumberToIntTransaction.WaitForResponseAsync(cts.Token);

            RequestDelayed();

            var responseValue = await responseValueTask;
            
            Assert.AreEqual(7, responseValue);

            async void RequestDelayed()
            {
                try
                {
                    await Task.Delay(100, cts.Token);
                    var respValue = await testNumberToIntTransaction.RequestAsync(NumberEnum.Seven, cts.Token);
                    Assert.AreEqual(7, respValue);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDownR3()
        {
            Object.DestroyImmediate(testNumberToIntTransaction);
        }
        
        private async ValueTask<string> FromNumberEnumAsync(NumberEnum number)
        {
            return await Task.FromResult(number.ToString());
        }
        
        private class NumberToIntTransaction : Transaction<NumberEnum, int>
        {
            protected override void RegisterResponseInternal()
            {
                RegisterResponse(ResponseInternal);
            }
            
            private async ValueTask<int> ResponseInternal(NumberEnum request, CancellationToken token)
            {
                await Task.Delay(100, token);
                return request switch
                {
                    NumberEnum.Zero => 0,
                    NumberEnum.One => 1,
                    NumberEnum.Two => 2,
                    NumberEnum.Three => 3,
                    NumberEnum.Four => 4,
                    NumberEnum.Five => 5,
                    NumberEnum.Six => 6,
                    NumberEnum.Seven => 7,
                    NumberEnum.Eight => 8,
                    NumberEnum.Nine => 9,
                    NumberEnum.Ten => 10,
                    _ => throw new ArgumentOutOfRangeException(nameof(request), request, null)
                };
            }
        }
    }
}

#endif
