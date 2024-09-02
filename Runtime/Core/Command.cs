using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Soar.Commands
{
    public abstract class Command : SoarCore
    {
        public abstract void Execute();

        // NOTE : ExecuteAsync is implemented as virtual to let override optional in derived class.
        public virtual async ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
            
            Execute();
            await Task.CompletedTask;
        }

        public override void Dispose() { }
    }

    public abstract class Command<T> : Command
    {
        public abstract void Execute(T param);
        
        public override void Execute()
        {
            Execute(default);
        }

        // NOTE : ExecuteAsync is implemented as virtual to let override optional in derived class.
        public virtual async ValueTask ExecuteAsync(T param, CancellationToken cancellationToken = default)
        {
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            var token = linkedTokenSource.Token;
            token.ThrowIfCancellationRequested();
            
            Execute(param);
            await Task.CompletedTask;
        }
    }
}