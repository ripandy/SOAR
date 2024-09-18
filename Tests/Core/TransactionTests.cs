using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Soar.Transactions.Tests
{
    public partial class TransactionTests
    {
        private Transaction testTransaction;
        private FloatTransaction testFloatTransaction;
        private NumberToStringTransaction testNumberToStringTransaction;
        private CircleAreaTransaction testCircleAreaTransaction;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testTransaction = ScriptableObject.CreateInstance<Transaction>();
            testFloatTransaction = ScriptableObject.CreateInstance<FloatTransaction>();
            testNumberToStringTransaction = ScriptableObject.CreateInstance<NumberToStringTransaction>();
            testCircleAreaTransaction = ScriptableObject.CreateInstance<CircleAreaTransaction>();
        }

        [SetUp]
        public void Setup()
        {
            testNumberToStringTransaction.ResetResponseInternal();
            testCircleAreaTransaction.ResetResponseInternal();
        }

        [Test]
        public void PlainTransaction_VariousResponse_ShouldRespondToPlainRequest()
        {
            var respondedCount = 0;
            testTransaction.RegisterResponse(() => respondedCount++);
            testFloatTransaction.RegisterResponse(ResponseAsync);
            
            var responseCount = 0;
            testTransaction.Request(() => responseCount++);
            testFloatTransaction.Request(() => responseCount++);
            testNumberToStringTransaction.Request(() => responseCount++);
            testCircleAreaTransaction.Request(() => responseCount++);
            
            Assert.AreEqual(4, responseCount, "Response Count is not equal to number of requests.");
            Assert.AreEqual(2, respondedCount, "Responded Count is not equal to number of manually registered response.");
            
            async ValueTask ResponseAsync()
            {
                await Task.CompletedTask;
                respondedCount++;
            }
        }
        
        [Test]
        public async Task PlainTransaction_VariousResponse_ShouldRespondToAsyncRequests()
        {
            var respondedCount = 0;
            testFloatTransaction.RegisterResponse(() => respondedCount++);
            testTransaction.RegisterResponse(ResponseAsync);
            
            await testTransaction.RequestAsync();
            await testFloatTransaction.RequestAsync();
            await testNumberToStringTransaction.RequestAsync();
            await testCircleAreaTransaction.RequestAsync();
            
            Assert.AreEqual(2, respondedCount, "Responded Count is not equal to number of manually registered response.");
            
            async ValueTask ResponseAsync()
            {
                await Task.CompletedTask;
                respondedCount++;
            }
        }
        
        [Test]
        public void ValueTransaction_PlainResponse_ShouldRespondToValueRequests()
        {
            var respondedCount = 0;
            testFloatTransaction.RegisterResponse(ResponseAsync);
            testCircleAreaTransaction.RegisterResponse(() => respondedCount++);
            testNumberToStringTransaction.RegisterResponse(() => respondedCount++);
            
            var floatResponseValue = 99f;
            var circleAreaResponseValue = 314f;
            var stringResponseValue = "Not Empty";
            testFloatTransaction.Request(42f, resp => floatResponseValue = resp);
            testCircleAreaTransaction.Request(4, resp => circleAreaResponseValue = resp);
            testNumberToStringTransaction.Request(NumberEnum.Eight, resp => stringResponseValue = resp);
            
            Assert.AreEqual(3, respondedCount, "Responded Count is not equal to number of manually registered response.");
            Assert.AreEqual(default(float), floatResponseValue, "Value Request to plain Response does not return default value.");
            Assert.AreEqual(default(float), circleAreaResponseValue, "Value Request to plain Response does not return default value.");
            Assert.AreEqual(default(string), stringResponseValue, "Value Request to plain Response does not return default value.");
            
            async ValueTask ResponseAsync()
            {
                await Task.CompletedTask;
                respondedCount++;
            }
        }
        
        [Test]
        public async Task ValueTransaction_PlainResponse_ShouldRespondToAsyncValueRequests()
        {
            var respondedCount = 0;
            testFloatTransaction.RegisterResponse(() => respondedCount++);
            testCircleAreaTransaction.RegisterResponse(ResponseAsync);
            testNumberToStringTransaction.RegisterResponse(ResponseAsync);
            
            var floatResponseValue = await testFloatTransaction.RequestAsync(42f);
            var circleAreaResponseValue = await testCircleAreaTransaction.RequestAsync(4);
            var stringResponseValue = await testNumberToStringTransaction.RequestAsync(NumberEnum.Eight);
            
            Assert.AreEqual(3, respondedCount, "Responded Count is not equal to number of manually registered response.");
            Assert.AreEqual(default(float), floatResponseValue, "Value Async Request to plain Response does not return default value.");
            Assert.AreEqual(default(float), circleAreaResponseValue, "Value Async Request to plain Response does not return default value.");
            Assert.AreEqual(default(string), stringResponseValue, "Value Async Request to plain Response does not return default value.");
            
            async ValueTask ResponseAsync()
            {
                await Task.CompletedTask;
                respondedCount++;
            }
        }

        [TestCase(NumberEnum.One)]
        [TestCase(NumberEnum.Three)]
        [TestCase(NumberEnum.Five)]
        public async Task ValueTransaction_RegisterResponse_ShouldHandleRegistrationProperly(NumberEnum requestValue)
        {
            // Should be responded with internal response
            var responseValue = await testNumberToStringTransaction.RequestAsync(requestValue);
            Assert.AreEqual(requestValue.ToString(), responseValue, "Internally Registered response does not work properly.");
            
            testNumberToStringTransaction.Request(requestValue, resp => responseValue = $"{resp}_A");
            Assert.AreEqual($"{requestValue}_A", responseValue, "Internally Registered response does not work properly.");
            
            // Internally registered response should be replaced
            testNumberToStringTransaction.RegisterResponse(ToStringAsync);
            
            responseValue = await testNumberToStringTransaction.RequestAsync(requestValue);
            Assert.AreEqual($"{requestValue}_B", responseValue, "Registered response does not get replaced.");
            
            testNumberToStringTransaction.Request(requestValue, resp => responseValue = $"{resp}_C");
            Assert.AreEqual($"{requestValue}_B_C", responseValue, "Registered response does not get replaced.");
            
            // Unregister response. Should not respond.
            testNumberToStringTransaction.UnregisterResponse();

            var asyncRequest = testNumberToStringTransaction.RequestAsync(requestValue);
            testNumberToStringTransaction.Request(requestValue, resp => responseValue = resp);
            Assert.AreEqual($"{requestValue}_B_C", responseValue, "Response unregistered, but responseValue is updated.");
            
            // Re-register with plain response.
            var respondedCount = 0;
            testNumberToStringTransaction.RegisterResponse(() => respondedCount++);
            Assert.AreEqual(default(string), responseValue, "Response re-registered, but responseValue have not been updated.");
            Assert.AreEqual(2, respondedCount, "Response Count is not equal to number of manually registered response.");

            // Unregistered before asyncRequest's await. asyncRequest Should be responded with plain response.
            testNumberToStringTransaction.UnregisterResponse();
            
            var otherAsyncRequest = testNumberToStringTransaction.RequestAsync(requestValue);
            
            responseValue = await asyncRequest;
            Assert.AreEqual(default(string), responseValue, "Response unregistered before await, but does not get responded.");
            
            testNumberToStringTransaction.Request(requestValue, resp => responseValue = resp);
            
            // Reset Internal Response
            testNumberToStringTransaction.ResetResponseInternal();
            Assert.AreEqual(requestValue.ToString(), responseValue, "Internal Response does not Re-registered properly.");

            responseValue = await otherAsyncRequest;
            Assert.AreEqual(requestValue.ToString(), responseValue, "Internal Response does not Re-registered properly.");
            
            // Override response with plain response async
            testNumberToStringTransaction.RegisterResponse(ResponseAsync);
            
            responseValue = await testNumberToStringTransaction.RequestAsync(requestValue);
            Assert.AreEqual(default(string), responseValue, "Registered response does not get replaced.");
            
            testNumberToStringTransaction.Request(requestValue, resp => responseValue = resp);
            Assert.AreEqual(default(string), responseValue, "Registered response does not get replaced.");
            
            Assert.AreEqual(4, respondedCount, "Response Count is not equal to number of manually registered response.");
            
            async ValueTask<string> ToStringAsync(NumberEnum number)
            {
                await Task.CompletedTask;
                return $"{number.ToString()}_B";
            }
            
            async ValueTask ResponseAsync()
            {
                await Task.CompletedTask;
                respondedCount++;
            }
        }
        
        [TestCase(2)]
        [TestCase(42)]
        [TestCase(100)]
        public void ValueTransaction_SubscribeToRequest_ShouldBeSubscribed(int requestValue)
        {
            var requested = false;
            var requestedValue = default(int);
            using var subscriptionToRequest = testCircleAreaTransaction.SubscribeToRequest(() => requested = true);
            using var subscriptionToValueRequest = testCircleAreaTransaction.SubscribeToRequest(req => requestedValue = req);
            
            var responseValue = default(float);
            testCircleAreaTransaction.Request(requestValue, response => responseValue = response);
            
            Assert.IsTrue(requested, "Request was not subscribed.");
            Assert.AreEqual(requestValue, requestedValue, "Request value was not subscribed.");
            Assert.AreEqual(Mathf.PI * requestValue * requestValue, responseValue, "Request was not responded.");
        }
        
        [TestCase(4)]
        [TestCase(24)]
        [TestCase(99)]
        public void ValueTransaction_SubscribeToResponse_ShouldBeSubscribed(int requestValue)
        {
            var responded = false;
            var respondedValue = default(float);
            using var subscriptionToResponse = testCircleAreaTransaction.SubscribeToResponse(() => responded = true);
            using var subscriptionToValueResponse = testCircleAreaTransaction.SubscribeToResponse(resp => respondedValue = resp);
            
            var responseValue = default(float);
            testCircleAreaTransaction.Request(requestValue, response => responseValue = response);
            
            Assert.IsTrue(responded, "Response was not subscribed.");
            Assert.AreEqual(responseValue, respondedValue, "Response value was not subscribed.");
            Assert.AreEqual(Mathf.PI * requestValue * requestValue, responseValue, "Request was not responded.");
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testTransaction);
            Object.DestroyImmediate(testFloatTransaction);
            Object.DestroyImmediate(testNumberToStringTransaction);
            Object.DestroyImmediate(testCircleAreaTransaction);
        }

        private class NumberToStringTransaction : Transaction<NumberEnum, string>
        {
            protected override void RegisterResponseInternal()
            {
                RegisterResponse(ResponseInternal);
            }
            
            private static string ResponseInternal(NumberEnum request)
            {
                return request.ToString();
            }
        }

        private class CircleAreaTransaction : Transaction<int, float>
        {
            protected override void RegisterResponseInternal()
            {
                RegisterResponse(GetArea);
            }

            private static async ValueTask<float> GetArea(int radius)
            {
                await Task.CompletedTask;
                return Mathf.PI * radius * radius;
            }
        }
    }
}