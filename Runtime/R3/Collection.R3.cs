#if SOAR_R3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using R3;
using UnityEngine;

namespace Soar.Collections
{
    public abstract partial class Collection<T>
    {
        internal readonly Subject<T> onAddSubject = new();
        internal readonly Subject<T> onRemoveSubject = new();
        private readonly Subject<object> onClearSubject = new();
        private readonly Subject<int> countSubject = new();
        private readonly Subject<IndexValuePair<T>> valueSubject = new();

        public Observable<T> ObserveAdd() => onAddSubject;
        public Observable<T> ObserveRemove() => onRemoveSubject;
        public Observable<Unit> ObserveClear() => onClearSubject.AsUnitObservable();
        public Observable<int> ObserveCount() => countSubject;
        public Observable<IndexValuePair<T>> ObserveValues() => valueSubject;
        public Observable<T> ObserveValues(int index) => valueSubject.Where(pair => pair.Index == index).Select(pair => pair.Value);

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
            await onClearSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
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
        
        public async ValueTask<T> ValuesAsync(int index, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ObserveValues(index).FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }

        internal partial void RaiseOnAdd(T addedValue)
        {
            onAddSubject.OnNext(addedValue);
        }

        internal partial void RaiseOnRemove(T removedValue)
        {
            onRemoveSubject.OnNext(removedValue);
        }

        internal partial void RaiseCount()
        {
            countSubject.OnNext(Count);
        }

        internal partial void RaiseValueAt(int index, T value)
        {
            if (valueEventType == ValueEventType.OnChange && list[index].Equals(value)) return;
            valueSubject.OnNext(new IndexValuePair<T>(index, value));
        }

        private partial void RaiseOnClear()
        {
            onClearSubject.OnNext(this);
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
    
    // List
    public abstract partial class List<T>
    {
        private readonly Subject<MovedValueDto<T>> moveSubject = new();
        private readonly Subject<IndexValuePair<T>> insertSubject = new();

        public Observable<MovedValueDto<T>> ObserveMove() => moveSubject;
        public Observable<IndexValuePair<T>> ObserveInsert() => insertSubject;
        
        public async ValueTask<MovedValueDto<T>> OnMoveAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await moveSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<IndexValuePair<T>> OnInsertAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await insertSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        private partial void RaiseOnMove(T value, int oldIndex, int newIndex)
        {
            moveSubject.OnNext(new MovedValueDto<T>(value, oldIndex, newIndex));
        }
        
        private partial void RaiseOnInsert(int index, T value)
        {
            insertSubject.OnNext(new IndexValuePair<T>(index, value));
        }
        
        public partial IDisposable SubscribeOnMove(Action<T, int, int> action)
        {
            return moveSubject.Subscribe(dto => action.Invoke(dto.Value, dto.OldIndex, dto.NewIndex));
        }
        
        public partial IDisposable SubscribeOnMove(Action<MovedValueDto<T>> action)
        {
            return moveSubject.Subscribe(action);
        }
        
        public partial IDisposable SubscribeOnInsert(Action<int, T> action)
        {
            return insertSubject.Subscribe(pair => action.Invoke(pair.Index, pair.Value));
        }
        
        public partial IDisposable SubscribeOnInsert(Action<IndexValuePair<T>> action)
        {
            return insertSubject.Subscribe(action);
        }

        public override void Dispose()
        {
            base.Dispose();
            moveSubject.Dispose();
            insertSubject.Dispose();
        }
    }
    
    // Dictionary
    public abstract partial class Dictionary<TKey, TValue>
    {
        private readonly Subject<KeyValuePair<TKey, TValue>> valueSubject = new();
        
        public new Observable<KeyValuePair<TKey, TValue>> ObserveValues() => valueSubject;

        public Observable<TValue> ObserveValues(TKey key) => valueSubject.Where(pair => pair.Key.Equals(key)).Select(pair => pair.Value);
             
        public new async ValueTask<KeyValuePair<TKey, TValue>> ValuesAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await valueSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
             
        public async ValueTask<TValue> ValuesAsync(TKey key, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ObserveValues(key).FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
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
        
        public IDisposable SubscribeToValues(TKey key, Action<TValue> action)
        {
            return ObserveValues(key).Subscribe(action);
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