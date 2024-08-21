#if SOAR_R3

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace Soar.Events.Tests
{
    /// <summary>
    /// Tests for the core functionality of the GameEvent class.
    /// Tests should pass for both independent implementation and R3 integration.
    /// </summary>
    public partial class GameEventTests
    {
        [Test]
        public async Task EventAsync_ShouldBeAwaited()
        {
            var task = testGameEvent.EventAsync();
            testGameEvent.Raise();
            await task;
            Assert.Pass();
            
            var intEventAsyncTask = testIntGameEvent.EventAsync();
            testIntGameEvent.Raise(42);
            var eventAsyncInt = await intEventAsyncTask;
            Assert.AreEqual(42, eventAsyncInt);
            
            var poseEventAsyncTask = testPoseGameEvent.EventAsync();
            testPoseGameEvent.Raise(new Pose(Vector3.right, Quaternion.Euler(Vector3.left)));
            var eventAsyncPose = await poseEventAsyncTask;
            Assert.AreEqual(new Pose(Vector3.right, Quaternion.Euler(Vector3.left)), eventAsyncPose);
        }
        
        [Test]
        public void EventAsync_ShouldThrowIfCancelled()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await testGameEvent.EventAsync(cts.Token));
            Assert.ThrowsAsync<TaskCanceledException>(async () => await testIntGameEvent.EventAsync(cts.Token));
            Assert.ThrowsAsync<TaskCanceledException>(async () => await testPoseGameEvent.EventAsync(cts.Token));
        }
    }
}

#endif
