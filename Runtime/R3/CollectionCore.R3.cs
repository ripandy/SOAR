#if SOAR_R3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using R3;
using UnityEngine;

namespace Soar.Collections
{
    // List
    public abstract partial class Collection<T>
    {
        protected readonly Subject<T> onAddSubject = new();
        protected readonly Subject<T> onRemoveSubject = new();
        private readonly Subject<object> onClearSubject = new();
        private readonly Subject<int> countSubject = new();
        private readonly Subject<IndexValuePair<T>> valueSubject = new();

        public Observable<T> ObserveAdd() => onAddSubject;
        public Observable<T> ObserveRemove() => onRemoveSubject;
        public Observable<Unit> ObserveClear() => onClearSubject.AsUnitObservable();
        public Observable<int> ObserveCount() => countSubject;
        public Observable<IndexValuePair<T>> ObserveValues() => valueSubject;
        
        public async ValueTask<T> OnAddAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await onAddSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<T> OnRemoveAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await onRemoveSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask OnClearAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            await onClearSubject.WaitAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await countSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<IndexValuePair<T>> ValuesAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await valueSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        private partial void RaiseOnAdd(T addedValue)
        {
            onAddSubject.OnNext(addedValue);
        }

        private partial void RaiseOnRemove(T removedValue)
        {
            onRemoveSubject.OnNext(removedValue);
        }

        private partial void RaiseOnClear()
        {
            onClearSubject.OnNext(this);
        }

        private partial void RaiseCount()
        {
            countSubject.OnNext(Count);
        }

        private partial void RaiseValueAt(int index, T value)
        {
            if (valueEventType == ValueEventType.OnChange && list[index].Equals(value)) return;
            valueSubject.OnNext(new IndexValuePair<T>(index, value));
        }

        public partial IDisposable SubscribeOnAdd(Action<T> action)
        {
            return onAddSubject.Subscribe(action);
        }

        public partial IDisposable SubscribeOnRemove(Action<T> action)
        {
            return onRemoveSubject.Subscribe(action);
        }

        public partial IDisposable SubscribeOnClear(Action action)
        {
            return onClearSubject.Subscribe(_ => action.Invoke());
        }
        
        public partial IDisposable SubscribeToCount(Action<int> action)
        {
            return countSubject.Subscribe(action);
        }

        public partial IDisposable SubscribeToValues(Action<int, T> action)
        {
            return valueSubject.Subscribe(pair => action.Invoke(pair.Index, pair.Value));
        }

        public partial IDisposable SubscribeToValues(Action<IndexValuePair<T>> action)
        {
            return valueSubject.Subscribe(action);
        }

        public override void Dispose()
        {
            onAddSubject.Dispose();
            onRemoveSubject.Dispose();
            onClearSubject.Dispose();
            countSubject.Dispose();
            valueSubject.Dispose();
        }
    }
    
    // Dictionary
    public abstract partial class Collection<TKey, TValue>
    {
        private readonly Subject<KeyValuePair<TKey, TValue>> valueSubject = new();
        
        public new Observable<KeyValuePair<TKey, TValue>> ObserveValues() => valueSubject;
        
        public new async ValueTask<KeyValuePair<TKey, TValue>> ValuesAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await valueSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }

        public partial IDisposable SubscribeOnAdd(Action<TKey, TValue> action)
        {
            return onAddSubject.Subscribe(pair => action.Invoke(pair.Key, pair.Value));
        }
        
        public partial IDisposable SubscribeOnRemove(Action<TKey, TValue> action)
        {
            return onRemoveSubject.Subscribe(pair => action.Invoke(pair.Key, pair.Value));
        }
    
        public partial IDisposable SubscribeToValues(Action<TKey, TValue> action)
        {
            return valueSubject.Subscribe(pair => action.Invoke(pair.Key, pair.Value));
        }

        public partial IDisposable SubscribeToValues(Action<KeyValuePair<TKey, TValue>> action)
        {
            return valueSubject.Subscribe(action);
        }

        private partial void RaiseValue(TKey key, TValue value)
        {
            if (valueEventType == ValueEventType.OnChange && IsValueEqual()) return;
            
            valueSubject.OnNext(new KeyValuePair<TKey, TValue>(key, value));

            bool IsValueEqual()
            {
                return dictionary.TryGetValue(key, out var val) && val.Equals(value);
            }
        }

        public override void Dispose()
        {
            valueSubject.Dispose();
            base.Dispose();
        }
    }
}

#endif