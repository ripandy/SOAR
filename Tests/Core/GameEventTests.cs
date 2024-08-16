using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Soar.Events.Tests
{
    public class GameEventTests
    {
        private GameEvent testGameEvent;
        private IntGameEvent testIntGameEvent;
        private PoseGameEvent testPoseGameEvent;

        private readonly List<IDisposable> subscriptions = new();
        
        private bool isRaised;
        private int raisedInt;
        private Pose raisedPose;
        private int raisedCount;

        [SetUp]
        public void Setup()
        {
            testGameEvent = ScriptableObject.CreateInstance<GameEvent>();
            testIntGameEvent = ScriptableObject.CreateInstance<IntGameEvent>();
            testPoseGameEvent = ScriptableObject.CreateInstance<PoseGameEvent>();

            var s1 = testGameEvent.Subscribe(() =>
            {
                isRaised = true;
                raisedCount++;
            }); 
            var s2 = testIntGameEvent.Subscribe(i => raisedInt = i);
            var s3 = testPoseGameEvent.Subscribe(p => raisedPose = p);
            
            var s4 = testIntGameEvent.Subscribe(() => raisedCount++);
            var s5 = testPoseGameEvent.Subscribe(() => raisedCount++);
            
            subscriptions.Add(s1);
            subscriptions.Add(s2);
            subscriptions.Add(s3);
            subscriptions.Add(s4);
            subscriptions.Add(s5);
        }
        
        [Test]
        public void RaiseAndSubscriptionPair_ShouldBeCalled()
        {
            isRaised = false;
            raisedInt = 0;
            raisedPose = default;
            var testPose = new Pose(Vector3.forward, Quaternion.Euler(Vector3.up));
            
            testGameEvent.Raise();
            testIntGameEvent.Raise(42);
            testPoseGameEvent.Raise(testPose);
            
            Assert.IsTrue(isRaised);
            Assert.AreEqual(42, raisedInt);
            Assert.AreEqual(testPose, raisedPose);
        }
        
        [Test]
        public void RaiseAndSubscriptionPair_AsBaseGameEvent_ShouldBeCalled()
        {
            raisedCount = 0;

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
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.Destroy(testGameEvent);
            Object.Destroy(testIntGameEvent);
            Object.Destroy(testPoseGameEvent);
            
            subscriptions.ForEach(s => s.Dispose());
        }
    }
}