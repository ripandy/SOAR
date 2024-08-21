#if SOAR_R3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using R3;
using UnityEngine;

namespace Soar.Events
{
    public partial class GameEvent
    {
        private readonly Subject<object> subject = new();
        
        // MEMO: SOAR uses `object` on parameterless GameEvent to avoid dependency on R3, whilst R3 use `Unit` on "parameterless" Subject.
        // This measures implicit internal change when R3 is removed, which would cause error trying to find `Unit`.
        // `AsObservable` on parameterless `GameEvent` returns `Observable<Unit>` due to explicit calls.
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
            await subject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }

        public override void Dispose()
        {
            subject.Dispose();
        }
    }

    public abstract partial class GameEvent<T>
    {
        protected readonly Subject<T> ValueSubject = new();
        
        public new Observable<T> AsObservable()
        {
            return ValueSubject;
        }

        public Observable<Unit> AsUnitObservable()
        {
            return base.AsObservable();
        }

        public IObservable<T> AsSystemObservable()
        {
            return ValueSubject.AsSystemObservable();
        }

        public IAsyncEnumerable<T> ToAsyncEnumerable(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return ValueSubject.ToAsyncEnumerable(linkedTokenSource.Token);
        }

        public virtual partial void Raise(T valueToRaise)
        {
            value = valueToRaise;
            base.Raise();
            ValueSubject.OnNext(valueToRaise);
        }

        public partial IDisposable Subscribe(Action<T> action)
        {
            return ValueSubject.Subscribe(action);
        }

        public new async ValueTask<T> EventAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ValueSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public override void Dispose()
        {
            ValueSubject.Dispose();
            base.Dispose();
        }
    }
}

#endif
