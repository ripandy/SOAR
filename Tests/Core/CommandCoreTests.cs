using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace Soar.Commands.Tests
{
    public class CommandCoreTests
    {
        private class TestCommand : CommandCore
        {
            public bool Executed { get; set; }
            
            public override void Execute()
            {
                Executed = true;
            }

            public override async ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Delay(100, cancellationToken);
                Executed = true;
            }
        }
        
        private TestCommand testCommand;
        
        [OneTimeSetUp]
        public void Setup()
        {
            testCommand = ScriptableObject.CreateInstance<TestCommand>();
        }
        
        [Test]
        public void Execute_ShouldBeCalled()
        {
            testCommand.Executed = false;
            testCommand.Execute();
            Assert.IsTrue(testCommand.Executed);
        }

        [Test]
        public async Task ExecuteAsync_ShouldCallExecute()
        {
            testCommand.Executed = false;
            await testCommand.ExecuteAsync();
            Assert.IsTrue(testCommand.Executed);
        }
        
        [Test]
        public void ExecuteAsync_ShouldThrowIfCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await testCommand.ExecuteAsync(cts.Token));
        }
    }
    
    public class CommandCoreGenericTests
    {
        private class IntTestCommand : CommandCore<int>
        {
            public int Value { get; set; }
            
            public override void Execute(int param)
            {
                Value = param;
            }
            
            public override async ValueTask ExecuteAsync(int param, CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Delay(100, cancellationToken);
                Value = param;
            }
        }
        
        private IntTestCommand testIntCommand;
        
        [SetUp]
        public void Setup()
        {
            testIntCommand = ScriptableObject.CreateInstance<IntTestCommand>();
        }
        
        [Test]
        public void ExecuteWithParam_ShouldBeCalled()
        {
            testIntCommand.Value = 0;
            testIntCommand.Execute(24);
            Assert.AreEqual(24, testIntCommand.Value);
        }
        
        [Test]
        public void Execute_ShouldCallExecuteWithDefaultParam()
        {
            testIntCommand.Value = 999;
            testIntCommand.Execute();
            Assert.AreEqual(default(int), testIntCommand.Value);
        }
        
        [Test]
        public async Task ExecuteAsync_ShouldCallExecuteWithParam()
        {
            testIntCommand.Value = 1;
            await testIntCommand.ExecuteAsync(42);
            Assert.AreEqual(42, testIntCommand.Value);
        }

        [Test]
        public async Task ExecuteAsyncBaseClass_ShouldCallExecuteWithDefaultParam()
        {
            testIntCommand.Value = 1;
            await testIntCommand.ExecuteAsync();
            Assert.AreEqual(default(int), testIntCommand.Value);
        }
        
        [Test]
        public void ExecuteAsync_ShouldThrowIfCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await testIntCommand.ExecuteAsync(42, cts.Token));
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testIntCommand);
        }
    }
}