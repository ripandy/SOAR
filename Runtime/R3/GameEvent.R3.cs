#if SOAR_R3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using R3;
using UnityEngine;

namespace Soar.Event
{
    public partial class GameEvent
    {
        private readonly Subject<object> subject = new();
        
        public Observable<Unit> AsObservable()
        {
            return subject.AsUnitObservable();
        }
        
        public virtual partial void Raise()
        {
            subject.OnNext(this);
        }

        public partial IDisposable Subscribe(Action action)
        {
            return subject.Subscribe(_ => action.Invoke());
        }

        public async ValueTask EventAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            await subject.WaitAsync(cancellationToken: linkedTokenSource.Token);
        }

        public override void Dispose()
        {
            subject.Dispose();
        }
    }

    public abstract partial class GameEvent<T>
    {
        private readonly Subject<T> valueSubject = new();
        
        public new Observable<T> AsObservable()
        {
            return valueSubject;
        }

        public Observable<Unit> AsUnitObservable()
        {
            return base.AsObservable();
        }

        public IObservable<T> AsSystemObservable()
        {
            return valueSubject.AsSystemObservable();
        }

        public IAsyncEnumerable<T> ToAsyncEnumerable(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return valueSubject.ToAsyncEnumerable(linkedTokenSource.Token);
        }

        public virtual partial void Raise(T valueToRaise)
        {
            value = valueToRaise;
            base.Raise();
            valueSubject.OnNext(valueToRaise);
        }

        public partial IDisposable Subscribe(Action<T> action)
        {
            return valueSubject.Subscribe(action);
        }

        public new async ValueTask<T> EventAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await valueSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public override void Dispose()
        {
            valueSubject.Dispose();
            base.Dispose();
        }
    }
}

#endif
