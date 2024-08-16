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
        private readonly Subject<T> onAddSubject = new();
        private readonly Subject<T> onRemoveSubject = new();
        private readonly Subject<object> onClearSubject = new();
        private readonly Subject<int> countSubject = new();
        private readonly Dictionary<int, Subject<T>> valueSubjects = new();

        public Observable<T> OnAddObservable() => onAddSubject;
        public Observable<T> OnRemoveObservable() => onRemoveSubject;
        public Observable<object> OnClearObservable() => onClearSubject;
        public Observable<int> CountObservable() => countSubject;

        public Observable<T> ValueAtObservable(int index)
        {
            if (valueSubjects.TryGetValue(index, out var elementSubject)) return elementSubject;
            
            elementSubject = new Subject<T>();
            valueSubjects.Add(index, elementSubject);

            return elementSubject;
        }
        
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
        
        public async ValueTask<T> ValueAtAsync(int index, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ValueAtObservable(index).FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
    }

    public abstract partial class Collection<TKey, TValue>
    {
        private readonly Dictionary<TKey, Subject<TValue>> valueSubjects = new();
        
        public Observable<TValue> ValueAtObservable(TKey key)
        {
            if (valueSubjects.TryGetValue(key, out var elementSubject)) return elementSubject;
            
            elementSubject = new Subject<TValue>();
            valueSubjects.Add(key, elementSubject);

            return elementSubject;
        }
        
        public async ValueTask<TValue> ValueAtAsync(TKey key, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ValueAtObservable(key).FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
    }
    
    public abstract partial class Collection<T>
    {
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
            if (!valueSubjects.TryGetValue(index, out var subject)) return;
            
            subject.OnNext(value);
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

        public partial IDisposable SubscribeToValueAt(int index, Action<T> action)
        {
            return ValueAtObservable(index).Subscribe(action);
        }
        
        private partial void IncrementValueSubscriptions(int index)
        {
            for (var i = list.Count; i > index; i--)
            {
                valueSubjects.TryChangeKey(i - 1, i);
            }
        }
        
        private partial void SwitchValueSubscription(int oldIndex, int newIndex)
        {
            valueSubjects.TryChangeKey(oldIndex, newIndex);
        }
        
        private partial void ClearValueSubscriptions()
        {
            foreach (var subject in valueSubjects.Values)
            {
                subject.Dispose();
            }
            
            valueSubjects.Clear();
        }
        
        private partial void RemoveValueSubscription(int index)
        {
            if (!valueSubjects.TryGetValue(index, out var subject)) return;
            
            subject.Dispose();
            valueSubjects.Remove(index);
        }

        private partial void DisposeSubscriptions()
        {
            onAddSubject.Dispose();
            onRemoveSubject.Dispose();
            onClearSubject.Dispose();
            countSubject.Dispose();
            ClearValueSubscriptions();
        }
    }
    
    public abstract partial class Collection<TKey, TValue>
    {
        public partial IDisposable SubscribeToValue(TKey key, Action<TValue> action)
        {
            return ValueAtObservable(key).Subscribe(action);
        }

        private partial void RaiseValue(TKey key, TValue value)
        {
            if (valueEventType == ValueEventType.OnChange && IsValueEqual()) return;
            if (!valueSubjects.TryGetValue(key, out var subject)) return;
            
            subject.OnNext(value);

            bool IsValueEqual()
            {
                return dictionary.TryGetValue(key, out var val) && val.Equals(value);
            }
        }

        private partial void ClearValueSubscriptions()
        {
            foreach (var subject in valueSubjects.Values)
            {
                subject.Dispose();
            }
            
            valueSubjects.Clear();
        }
        
        private partial void RemoveValueSubscription(TKey key)
        {
            if (!valueSubjects.TryGetValue(key, out var subject)) return;
            
            subject.Dispose();
            valueSubjects.Remove(key);
        }
    }
}

#endif