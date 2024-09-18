#if SOAR_R3

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Soar.Transactions.Tests
{
    public partial class TransactionTests
    {
        private NumberToIntTransaction testNumberToIntTransaction;
        
        [OneTimeSetUp]
        public void SetupR3()
        {
            testNumberToIntTransaction = ScriptableObject.CreateInstance<NumberToIntTransaction>();
        }
        
        // TODO: add tests around Cancellation Token
        [TestCase(NumberEnum.Five)]
        [TestCase(NumberEnum.Seven)]
        [TestCase(NumberEnum.Nine)]
        public async Task NumberToStringTransaction_RegisterResponseWithCancellationToken_ShouldBeCancelledAccordingly(NumberEnum requestValue)
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
        
        // TODO: add tests around Observable Conversion
        
        [OneTimeTearDown]
        public void TearDownR3()
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
