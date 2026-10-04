using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Soar.Events.Tests
{
    /// <summary>
    /// Tests for the core functionality of the GameEvent class.
    /// Tests should pass for both independent implementation and R3 integration.
    /// </summary>
    public partial class GameEventTests
    {
        private GameEvent testGameEvent;
        private IntGameEvent testIntGameEvent;
        private PoseGameEvent testPoseGameEvent;
        
        [OneTimeSetUp]
        public void Setup()
        {
            testGameEvent = ScriptableObject.CreateInstance<GameEvent>();
            testIntGameEvent = ScriptableObject.CreateInstance<IntGameEvent>();
            testPoseGameEvent = ScriptableObject.CreateInstance<PoseGameEvent>();
        }
        
        [Test]
        public void RaiseAndSubscriptionPair_ShouldBeCalled()
        {
            var isRaised = false;
            var raisedInt = 0;
            var raisedPose = Pose.identity;
            var testPose = new Pose(Vector3.forward, Quaternion.Euler(Vector3.up));
            
            var s1 = testGameEvent.Subscribe(() => isRaised = true); 
            var s2 = testIntGameEvent.Subscribe(i => raisedInt = i);
            var s3 = testPoseGameEvent.Subscribe(p => raisedPose = p);
            
            testGameEvent.Raise();
            testIntGameEvent.Raise(42);
            testPoseGameEvent.Raise(testPose);
            
            Assert.IsTrue(isRaised);
            Assert.AreEqual(42, raisedInt);
            Assert.AreEqual(testPose, raisedPose);
            
            s1.Dispose();
            s2.Dispose();
            s3.Dispose();
            
            isRaised = false;
            var otherTestPose = new Pose(Vector3.back, Quaternion.Euler(Vector3.down));
            
            testGameEvent.Raise();
            testIntGameEvent.Raise(24);
            testPoseGameEvent.Raise(otherTestPose);
            
            // Should not be called after disposed.
            Assert.IsFalse(isRaised);
            Assert.AreEqual(42, raisedInt);
            Assert.AreEqual(testPose, raisedPose);
        }
        
        [Test]
        public void RaiseAndSubscriptionPair_AsBaseGameEvent_ShouldBeCalled()
        {
            var raisedCount = 0;
            
            var s1 = testGameEvent.Subscribe(() => raisedCount++);
            var s2 = testIntGameEvent.Subscribe(() => raisedCount++);
            var s3 = testPoseGameEvent.Subscribe(() => raisedCount++);

            var gameEvents = new[]
            {
                testGameEvent,
                testIntGameEvent,
                testPoseGameEvent
            };

            foreach (var gameEvent in gameEvents)
            {
                gameEvent.Raise();
            }
            
            Assert.AreEqual(3, raisedCount);
            
            s1.Dispose();
            s2.Dispose();
            s3.Dispose();
            
            foreach (var gameEvent in gameEvents)
            {
                gameEvent.Raise();
            }
            
            // Should not be called after disposed.
            Assert.AreEqual(3, raisedCount);
        }
        
        [Test]
        public void HandlerDisposingItselfDuringRaise_ShouldNotThrow()
        {
            var calls = 0;
            var typedCalls = 0;
            IDisposable self = null;
            IDisposable typedSelf = null;
            self = testGameEvent.Subscribe(() => self.Dispose());
            typedSelf = testIntGameEvent.Subscribe(_ => typedSelf.Dispose());
            var other = testGameEvent.Subscribe(() => calls++);
            var typedOther = testIntGameEvent.Subscribe(_ => typedCalls++);
            
            Assert.DoesNotThrow(() => testGameEvent.Raise(), "Disposing a subscription from inside its own handler should not throw.");
            Assert.DoesNotThrow(() => testIntGameEvent.Raise(1), "Disposing a typed subscription from inside its own handler should not throw.");
            
            testGameEvent.Raise();
            testIntGameEvent.Raise(2);
            Assert.AreEqual(2, calls, "Remaining subscriber should run on both raises.");
            Assert.AreEqual(2, typedCalls, "Remaining typed subscriber should run on both raises.");
            
            other.Dispose();
            typedOther.Dispose();
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testGameEvent);
            Object.DestroyImmediate(testIntGameEvent);
            Object.DestroyImmediate(testPoseGameEvent);
        }
    }
}