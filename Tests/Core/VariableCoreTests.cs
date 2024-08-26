using NUnit.Framework;
using Soar.Events;
using UnityEngine;

namespace Soar.Variables.Tests
{
    /// <summary>
    /// Tests for the core functionality of the VariableCore class.
    /// Tests should pass for both independent implementation and R3 integration. 
    /// </summary>
    /// TODO: Add tests for R3 integration.
    public class VariableCoreTests
    {
        private IntVariable testIntVariable;
        private Vector3Variable testVector3Variable;
        
        [OneTimeSetUp]
        public void Setup()
        {
            testIntVariable = ScriptableObject.CreateInstance<IntVariable>();
            testVector3Variable = ScriptableObject.CreateInstance<Vector3Variable>();
        }

        [Test]
        public void ValueEventAndSubscription_ShouldBeCalled()
        {
            var eventInt = 0;
            var eventVector3 = Vector3.zero;
            
            var s1 = testIntVariable.Subscribe(value => eventInt = value);
            var s2 = testVector3Variable.Subscribe(vector3 => eventVector3 = vector3);

            testIntVariable.Value = 42;
            testVector3Variable.Value = Vector3.forward;
            
            Assert.AreEqual(42, eventInt);
            Assert.AreEqual(Vector3.forward, eventVector3);
            
            // MEMO: Assert.AreEqual checks for type equality. Indirect cast would fail.
            Assert.IsTrue(testIntVariable == eventInt, "Should be equal with indirect cast");
            Assert.IsTrue(testVector3Variable == eventVector3, "Should be equal with indirect cast");
            
            s1.Dispose();
            s2.Dispose();

            testIntVariable.Value = 24;
            testVector3Variable.Value = Vector3.back;
            
            // Should not be called after disposed.
            Assert.AreEqual(42, eventInt);
            Assert.AreEqual(Vector3.forward, eventVector3);
            Assert.IsTrue(testIntVariable == 24);
            Assert.IsTrue(testVector3Variable == Vector3.back);
            Assert.IsFalse(testIntVariable == eventInt);
            Assert.IsFalse(testVector3Variable == eventVector3);
            
            var pairwiseIntValue = new PairwiseValue<int>(0, 0);
            var eventOldVector3 = Vector3.zero;
            
            var s3 = testIntVariable.Subscribe(value => pairwiseIntValue = value);
            var s4 = testVector3Variable.Subscribe((oldValue, newValue) =>
            {
                eventOldVector3 = oldValue;
                eventVector3 = newValue;
            });
            
            testIntVariable.Value = 420;
            testVector3Variable.Value = Vector3.up;
            
            Assert.AreEqual(24, pairwiseIntValue.OldValue);
            Assert.AreEqual(420, pairwiseIntValue.NewValue);
            Assert.AreEqual(Vector3.back, eventOldVector3);
            Assert.AreEqual(Vector3.up, eventVector3);
            Assert.IsTrue(testIntVariable == pairwiseIntValue.NewValue, "Should be equal with indirect cast");
            Assert.IsTrue(testVector3Variable == eventVector3, "Should be equal with indirect cast");
            
            s3.Dispose();
            s4.Dispose();
            
            testIntVariable.Value = 240;
            testVector3Variable.Value = Vector3.down;
            
            // Should not be called after disposed.
            Assert.AreEqual(24, pairwiseIntValue.OldValue);
            Assert.AreEqual(420, pairwiseIntValue.NewValue);
            Assert.AreEqual(Vector3.back, eventOldVector3);
            Assert.AreEqual(Vector3.up, eventVector3);
            Assert.IsTrue(testIntVariable == 240);
            Assert.IsTrue(testVector3Variable == Vector3.down);
            Assert.IsFalse(testIntVariable == pairwiseIntValue.NewValue);
            Assert.IsFalse(testVector3Variable == eventVector3);
        }
        
        [Test]
        public void ValueEventAndSubscription_AsGameEvent_ShouldBeCalled()
        {
            var eventInt = 0;
            var eventVector3 = Vector3.zero;
            
            var s1 = testIntVariable.Subscribe(value => eventInt = value);
            var s2 = testVector3Variable.Subscribe(value => eventVector3 = value);
            
            GameEvent<int> intGameEvent = testIntVariable;
            GameEvent<Vector3> vector3GameEvent = testVector3Variable;
            
            // Raised as GameEvent with parameter.
            intGameEvent.Raise(42);
            vector3GameEvent.Raise(Vector3.forward);
            
            Assert.AreEqual(42, eventInt);
            Assert.AreEqual(Vector3.forward, eventVector3);
            
            s1.Dispose();
            s2.Dispose();
            
            testIntVariable.Value = 24;
            testVector3Variable.Value = Vector3.back;
            
            // event values should not be changed.
            Assert.AreEqual(42, eventInt);
            Assert.AreEqual(Vector3.forward, eventVector3);
            
            // variable values should be changed.
            // MEMO: Assert.AreEqual checks for type equality. Indirect cast would fail.
            Assert.IsTrue(testIntVariable == 24);
            Assert.IsTrue(testVector3Variable == Vector3.back);
            
            var gameEvents = new GameEvent[]
            {
                testIntVariable,
                testVector3Variable
            };
            
            var isRaised = false;
            
            // re-subscribe.
            using var s3 = testIntVariable.Subscribe(value => eventInt = value);
            using var s4 = testVector3Variable.Subscribe(value => eventVector3 = value);
            using var s5 = testVector3Variable.Subscribe(() => isRaised = true);

            // Raised as GameEvent.
            foreach (var gameEvent in gameEvents)
            {
                gameEvent.Raise();
            }
            
            // event values should be current variable values.
            Assert.AreEqual(24, eventInt);
            Assert.AreEqual(Vector3.back, eventVector3);
            
            // parameterless subscription should be called.
            Assert.IsTrue(isRaised);
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            testIntVariable.Dispose();
            testVector3Variable.Dispose();
        }
    }
}