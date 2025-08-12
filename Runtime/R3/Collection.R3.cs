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
        private Subject<T> onAddSubject;
        internal Subject<T> OnAddSubject
        {
            get
            {
                if (onAddSubject == null || onAddSubject.IsDisposed)
                {
                    onAddSubject = new Subject<T>();
                }
                return onAddSubject;
            }
        }
        private Subject<T> onRemoveSubject;
        internal Subject<T> OnRemoveSubject
        {
            get
            {
                if (onRemoveSubject == null || onRemoveSubject.IsDisposed)
                {
                    onRemoveSubject = new Subject<T>();
                }
                return onRemoveSubject;
            }
        }
        private Subject<object> onClearSubject;
        private Subject<object> OnClearSubject
        {
            get
            {
                if (onClearSubject == null || onClearSubject.IsDisposed)
                {
                    onClearSubject = new Subject<object>();
                }
                return onClearSubject;
            }
        }
        private Subject<int> countSubject;
        private Subject<int> CountSubject
        {
            get
            {
                if (countSubject == null || countSubject.IsDisposed)
                {
                    countSubject = new Subject<int>();
                }
                return countSubject;
            }
        }
        private Subject<IndexValuePair<T>> valueSubject;
        private Subject<IndexValuePair<T>> ValueSubject
        {
            get
            {
                if (valueSubject == null || valueSubject.IsDisposed)
                {
                    valueSubject = new Subject<IndexValuePair<T>>();
                }
                return valueSubject;
            }
        }

        public Observable<T> ObserveAdd() => OnAddSubject;
        public Observable<T> ObserveRemove() => OnRemoveSubject;
        public Observable<Unit> ObserveClear() => OnClearSubject.AsUnitObservable();
        public Observable<int> ObserveCount() => CountSubject;
        public Observable<IndexValuePair<T>> ObserveValues() => ValueSubject;
        public Observable<T> ObserveValues(int index) => ValueSubject.Where(pair => pair.Index == index).Select(pair => pair.Value);

        public async ValueTask<T> OnAddAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await OnAddSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<T> OnRemoveAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await OnRemoveSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask OnClearAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            await OnClearSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await CountSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<IndexValuePair<T>> ValuesAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ValueSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<T> ValuesAsync(int index, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ObserveValues(index).FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }

        internal partial void RaiseOnAdd(T addedValue)
        {
            OnAddSubject.OnNext(addedValue);
        }

        internal partial void RaiseOnRemove(T removedValue)
        {
            OnRemoveSubject.OnNext(removedValue);
        }

        internal partial void RaiseCount()
        {
            CountSubject.OnNext(Count);
        }

        internal partial void RaiseValueAt(int index, T value)
        {
            ValueSubject.OnNext(new IndexValuePair<T>(index, value));
        }

        private partial void RaiseOnClear()
        {
            OnClearSubject.OnNext(this);
        }
        
        public partial IDisposable SubscribeOnAdd(Action<T> action)
        {
            return OnAddSubject.Subscribe(action);
        }

        public partial IDisposable SubscribeOnRemove(Action<T> action)
        {
            return OnRemoveSubject.Subscribe(action);
        }

        public partial IDisposable SubscribeOnClear(Action action)
        {
            return OnClearSubject.Subscribe(_ => action.Invoke());
        }
        
        public partial IDisposable SubscribeToCount(Action<int> action)
        {
            return CountSubject.Subscribe(action);
        }

        public partial IDisposable SubscribeToValues(Action<int, T> action)
        {
            return ValueSubject.Subscribe(pair => action.Invoke(pair.Index, pair.Value));
        }

        public partial IDisposable SubscribeToValues(Action<IndexValuePair<T>> action)
        {
            return ValueSubject.Subscribe(action);
        }

        public override void Dispose()
        {
            onAddSubject?.Dispose();
            onRemoveSubject?.Dispose();
            onClearSubject?.Dispose();
            countSubject?.Dispose();
            valueSubject?.Dispose();
        }
    }
    
    // List
    public abstract partial class SoarList<T>
    {
        private Subject<MovedValueDto<T>> moveSubject;
        private Subject<MovedValueDto<T>> MoveSubject
        {
            get
            {
                if (moveSubject == null || moveSubject.IsDisposed)
                {
                    moveSubject = new Subject<MovedValueDto<T>>();
                }
                return moveSubject;
            }
        }
        private Subject<IndexValuePair<T>> insertSubject;
        private Subject<IndexValuePair<T>> InsertSubject
        {
            get
            {
                if (insertSubject == null || insertSubject.IsDisposed)
                {
                    insertSubject = new Subject<IndexValuePair<T>>();
                }
                return insertSubject;
            }
        }

        public Observable<MovedValueDto<T>> ObserveMove() => MoveSubject;
        public Observable<IndexValuePair<T>> ObserveInsert() => InsertSubject;
        
        public async ValueTask<MovedValueDto<T>> OnMoveAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await MoveSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<IndexValuePair<T>> OnInsertAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await InsertSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        private partial void RaiseOnMove(T value, int oldIndex, int newIndex)
        {
            MoveSubject.OnNext(new MovedValueDto<T>(value, oldIndex, newIndex));
        }
        
        private partial void RaiseOnInsert(int index, T value)
        {
            InsertSubject.OnNext(new IndexValuePair<T>(index, value));
        }
        
        public partial IDisposable SubscribeOnMove(Action<T, int, int> action)
        {
            return MoveSubject.Subscribe(dto => action.Invoke(dto.Value, dto.OldIndex, dto.NewIndex));
        }
        
        public partial IDisposable SubscribeOnMove(Action<MovedValueDto<T>> action)
        {
            return MoveSubject.Subscribe(action);
        }
        
        public partial IDisposable SubscribeOnInsert(Action<int, T> action)
        {
            return InsertSubject.Subscribe(pair => action.Invoke(pair.Index, pair.Value));
        }
        
        public partial IDisposable SubscribeOnInsert(Action<IndexValuePair<T>> action)
        {
            return InsertSubject.Subscribe(action);
        }

        public override void Dispose()
        {
            base.Dispose();
            moveSubject?.Dispose();
            insertSubject?.Dispose();
        }
    }
    
    // Dictionary
    public abstract partial class SoarDictionary<TKey, TValue>
    {
        private Subject<KeyValuePair<TKey, TValue>> valueSubject;
        private Subject<KeyValuePair<TKey, TValue>> ValueSubject
        {
            get
            {
                if (valueSubject == null || valueSubject.IsDisposed)
                {
                    valueSubject = new Subject<KeyValuePair<TKey, TValue>>();
                }
                return valueSubject;
            }
        }
        
        public new Observable<KeyValuePair<TKey, TValue>> ObserveValues() => ValueSubject;

        public Observable<TValue> ObserveValues(TKey key) => ValueSubject.Where(pair => pair.Key.Equals(key)).Select(pair => pair.Value);
             
        public new async ValueTask<KeyValuePair<TKey, TValue>> ValuesAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ValueSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
             
        public async ValueTask<TValue> ValuesAsync(TKey key, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ObserveValues(key).FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public partial IDisposable SubscribeOnAdd(Action<TKey, TValue> action)
        {
            return OnAddSubject.Subscribe(pair => action.Invoke(pair.Key, pair.Value));
        }
        
        public partial IDisposable SubscribeOnRemove(Action<TKey, TValue> action)
        {
            return OnRemoveSubject.Subscribe(pair => action.Invoke(pair.Key, pair.Value));
        }
    
        public partial IDisposable SubscribeToValues(Action<TKey, TValue> action)
        {
            return ValueSubject.Subscribe(pair => action.Invoke(pair.Key, pair.Value));
        }

        public partial IDisposable SubscribeToValues(Action<KeyValuePair<TKey, TValue>> action)
        {
            return ValueSubject.Subscribe(action);
        }
        
        public IDisposable SubscribeToValues(TKey key, Action<TValue> action)
        {
            return ObserveValues(key).Subscribe(action);
        }
        
        private partial void RaiseValue(TKey key, TValue value)
        {
            ValueSubject.OnNext(new KeyValuePair<TKey, TValue>(key, value));
        }

        public override void Dispose()
        {
            valueSubject?.Dispose();
            base.Dispose();
        }
    }
}

#endif

